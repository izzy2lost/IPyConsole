# -*- coding: utf-8 -*-
# Python Script, API Version = V241

import clr

clr.AddReference("System")
clr.AddReference("System.IO")
clr.AddReference("System.Threading")
clr.AddReference("System.Drawing")
clr.AddReference("System.Windows.Forms")

from System import Type, Activator
from System.Drawing import Point as sysPoint, Size as sysSize
from System.Threading import Thread, ThreadStart, ApartmentState, CancellationTokenSource
from System.Windows.Forms import Application, Form, Label, TextBox, Button, OpenFileDialog, DialogResult, MessageBox, FolderBrowserDialog, MessageBoxButtons, GroupBox, RadioButton, CheckBox
from System.IO import File, Directory, FileInfo, Path

import sys
import re
import os
import traceback
import tempfile

class MainForm(Form):
    def __init__(self):
        # 创建self.XmpViewer
        try:
            # Virtual Huamn Vision Lab --- VisionLabViewer.Application
            # Virtual Photometric Lab --- XmpViewer.Application
            self.theApp = Type.GetTypeFromProgID("VisionLabViewer.Application")
            if self.theApp is None:
                raise Exception("请以管理员身份运行‘Virtual Huamn Vision Lab’!")
            self.XmpViewer = Activator.CreateInstance(self.theApp)
        except Exception as e:
            self.handleError("初始化错误", e)
            
        # 标志位-终止仿真
        self.stop_sim = False

        # 定义窗口
        self.Name = "自动化附材质并执行导出图片"
        self.Text = "自动化附材质并执行导出图片"
        self.Width = 1000
        self.Height = 300
        self.TopMost = True
        
        # 选择 surface library path
        self.labelSurfaceLibrary = Label()
        self.labelSurfaceLibrary.Text = "surface library path:"
        self.labelSurfaceLibrary.Location = sysPoint(30, 10)
        self.labelSurfaceLibrary.Size = sysSize(180, 20)
        self.textBoxSurfaceLibrary = TextBox()
        self.textBoxSurfaceLibrary.Location = sysPoint(230, 10)
        self.textBoxSurfaceLibrary.Size = sysSize(430, 30)
        self.btnChooseSurfacePath = Button()
        self.btnChooseSurfacePath.Text = "…"
        self.btnChooseSurfacePath.Location = sysPoint(670, 10)
        self.btnChooseSurfacePath.Size = sysSize(28, 20)
        self.btnChooseSurfacePath.Click += self.setSurfacePath
        
        # 选择 volume library path:
        self.labelVolumeLibrary = Label()
        self.labelVolumeLibrary.Text = "volume library path:"
        self.labelVolumeLibrary.Location = sysPoint(30, 50)
        self.labelVolumeLibrary.Size = sysSize(180, 20)
        self.textBoxVolumeLibrary = TextBox()
        self.textBoxVolumeLibrary.Location = sysPoint(230, 50)
        self.textBoxVolumeLibrary.Size = sysSize(430, 30)
        self.btnChooseVolumePath = Button()
        self.btnChooseVolumePath.Text = "…"
        self.btnChooseVolumePath.Location = sysPoint(670, 50)
        self.btnChooseVolumePath.Size = sysSize(28, 20)  
        self.btnChooseVolumePath.Click += self.setVolumePath
        
        # 选择 当前使用的scdocx 文件
        self.labelScDocxPath = Label()
        self.labelScDocxPath.Text = "scdocx file:"
        self.labelScDocxPath.Location = sysPoint(30, 90)
        self.labelScDocxPath.Size = sysSize(180, 20)
        self.textBoxScDocxPath = TextBox()
        self.textBoxScDocxPath.Location = sysPoint(230, 90)
        self.textBoxScDocxPath.Size = sysSize(430, 30)
        self.btnChooseScDocxPath = Button()
        self.btnChooseScDocxPath.Text = "…"
        self.btnChooseScDocxPath.Location = sysPoint(670, 90)
        self.btnChooseScDocxPath.Size = sysSize(28, 20)  
        self.btnChooseScDocxPath.Click += self.setScDocxPath
        
        # Radio组 单选CPU/GPU仿真
        self.groupBox = GroupBox()
        self.groupBox.Location = sysPoint(750, 10)
        self.groupBox.Size = sysSize(200, 80)
        self.groupBox.Text = "CPU/GPU"
        
        # Radio Button CPU
        self.radio_btn_cpu = RadioButton() 
        self.radio_btn_cpu.Text = "CPU"
        self.radio_btn_cpu.Location = sysPoint(10, 20)
        self.radio_btn_cpu.Checked = True  # 默认：CPU仿真
        
        # Radio Button GPU
        self.radio_btn_gpu = RadioButton() 
        self.radio_btn_gpu.Text = "GPU"
        self.radio_btn_gpu.Location = sysPoint(10, 40)
        
        # 终止选项
        self.label_pause = Label()
        self.label_pause.Text = "出现错误，自动终止"
        self.label_pause.Location = sysPoint(790, 110)
        self.label_pause.Size = sysSize(180, 20)
        self.checkbox_pause = CheckBox()
        self.checkbox_pause.Location = sysPoint(760, 100)
        self.checkbox_pause.Size = sysSize(30, 30)
        self.checkbox_pause.Checked = False  # 默认：遇到错误不中止程序
        
        # 按钮 执行仿真
        self.btnCompute = Button()
        self.btnCompute.Text = "开始执行"
        self.btnCompute.Location = sysPoint(350, 160)
        self.btnCompute.Size = sysSize(180, 30)
        self.btnCompute.Click += self.compute
        
        # 显示输出路径
        self.labelInfo = Label()
        self.labelInfo.Location = sysPoint(30, 270)
        self.labelInfo.Size = sysSize(340, 30)
        
        # 路径配置文件
        self.outpath = ""
        self.configFile = os.path.join(tempfile.gettempdir(), "zhanshengconfig.txt")
        self.errorlogFile = os.path.join(tempfile.gettempdir(), "zhanshengerrorlog.txt")

        # 从路径配置文件中读取上一次记录的文件
        if os.path.isfile(self.configFile):
            with open(self.configFile, "r") as f:
                count = 0
                for line in f.readlines():
                    count += 1
                    line = line.strip()
                    if count == 1:
                        self.textBoxSurfaceLibrary.Text = line
                    elif count == 2:
                        self.textBoxVolumeLibrary.Text = line
                    elif count == 3:
                        self.textBoxScDocxPath.Text = line
                    elif count == 4:
                        sim_type = line.split()[0]
                        self.radio_btn_cpu.Checked = (sim_type == "CPU")
                        self.radio_btn_gpu.Checked = (sim_type == "GPU")

        # 添加控件
        self.Controls.Add(self.labelSurfaceLibrary)
        self.Controls.Add(self.textBoxSurfaceLibrary)
        self.Controls.Add(self.btnChooseSurfacePath)
        self.Controls.Add(self.labelVolumeLibrary)
        self.Controls.Add(self.textBoxVolumeLibrary)
        self.Controls.Add(self.btnChooseVolumePath)
        self.Controls.Add(self.labelScDocxPath)
        self.Controls.Add(self.textBoxScDocxPath)
        self.Controls.Add(self.btnChooseScDocxPath)
        self.Controls.Add(self.btnCompute)
        self.Controls.Add(self.labelInfo)
        self.Controls.Add(self.groupBox)
        self.Controls.Add(self.label_pause)
        self.Controls.Add(self.checkbox_pause)
        self.groupBox.Controls.Add(self.radio_btn_cpu)
        self.groupBox.Controls.Add(self.radio_btn_gpu)
        
    def setSurfacePath(self, sender, event):
        dialog = FolderBrowserDialog()
        if dialog.ShowDialog() == DialogResult.OK:
            self.textBoxSurfaceLibrary.Text = dialog.SelectedPath

    def setVolumePath(self, sender, event):
        dialog = FolderBrowserDialog()
        if dialog.ShowDialog() == DialogResult.OK:
            self.textBoxVolumeLibrary.Text = dialog.SelectedPath
    
    def setScDocxPath(self, sender, event):
        dialog = OpenFileDialog()
        dialog.Filter = "scdocx files (*.scdocx)|*.scdocx"
        if dialog.ShowDialog() == DialogResult.OK:
            self.textBoxScDocxPath.Text = dialog.FileName
    
    def compute(self, sender, event):
        try: 
            self.labelInfo.Text = "开始执行仿真"
            self.computeAll()
        except Exception as e:
            self.handleError("执行仿真时发生错误", e)
    
    def computeAll(self):
        selectedSim = Selection.GetActive()
        if selectedSim == None or len(selectedSim.Items) != 1:
            MessageBox.Show("请选择一个simulation", "错误", MessageBoxButtons.OK)
            return
        
        # 写入路径配置文件
        with open(self.configFile, "w") as f:
            f.write(self.textBoxSurfaceLibrary.Text + "\n")
            f.write(self.textBoxVolumeLibrary.Text + "\n")
            f.write(self.textBoxScDocxPath.Text + "\n")
            # 写入CPU/GPU仿真
            if self.radio_btn_cpu.Checked:
                f.write(self.radio_btn_cpu.Text + " Compute" + "\n")
            else:
                f.write(self.radio_btn_gpu.Text + " Compute" + "\n")
        
        # 表面材质列表
        surfaceList = []
        if os.path.isdir(self.textBoxSurfaceLibrary.Text):
            surfaceList = getFiles(self.textBoxSurfaceLibrary.Text, [
                                    ".simplescattering",
                                    ".scattering",
                                    ".brdf",
                                    ".bsdf",
                                    ".bsdf180",
                                    ".coated",
                                    ".mirror",
                                    ".doe",
                                    ".fluorescent",
                                    ".grating",
                                    ".retroreflecting",
                                    ".anisotropic",
                                    ".polarizer",
                                    ".anisotropicbsdf",
                                    ".unpolished"
                                    ])
        else:
            print("未设置 Surface 路径!")
        
        # 体积材质列表
        volumeList = []
        if os.path.isdir(self.textBoxVolumeLibrary.Text):
            volumeList = getFiles(self.textBoxVolumeLibrary.Text, [
                                    ".material"])
        else:
            print("未设置 Volume 路径!")
        
        if len(surfaceList) == 0 and len(volumeList) == 0:
            MessageBox.Show("没有找到任何材质文件，请检查路径是否正确。", "错误", MessageBoxButtons.OK)
            return
        
        if os.path.isfile(self.textBoxScDocxPath.Text) == False:
            MessageBox.Show("没有找到 scdocx 文件，请检查路径是否正确。", "错误", MessageBoxButtons.OK)
            return
        
        line = self.textBoxScDocxPath.Text
        baseName = os.path.basename(line)
        theDir = os.path.dirname(line)
        projectName = os.path.splitext(baseName)[0]
        self.outPath = os.path.join(theDir,"SPEOS output files", projectName)        
        
        try:
            material = SpeosSim.Material.Find("MAINMATERIAL")
        except Exception as e:
            self.handleError("未找到材质异常", e)
            MessageBox.Show("请将材质命名为：MAINMATERIAL", "错误", MessageBoxButtons.OK)
            return
        
        self.computeSurface(selectedSim, material, surfaceList)
        self.computeVolume(selectedSim, material, volumeList)

        MessageBox.Show("执行结束!", "结束", MessageBoxButtons.OK)
        self.labelInfo.Text = ""
    
    #
    def computeSurface(self, selectedSim, material, surfaceList = []):
        if len(surfaceList) == 0:
            return False
        
        if self.checkbox_pause.Checked and self.stop_sim == True:
            return False
        
        # 设置 material 参数
        material.VOPType = SpeosSim.Material.EnumVOPType.Opaque
        material.SOPType = SpeosSim.Material.EnumSOPType.Library
        
        for surfacePath in surfaceList:
            self.labelInfo.Text = "正在使用：" + surfacePath
            material.SOPLibrary = surfacePath

            if not self.compute_and_check(selectedSim):
                # 终止仿真
                self.stop_sim = True
                break
            
            #SpeosSim.Command.ComputeOnActiveSelection()
            
            baseName = os.path.basename(surfacePath)
            imageName = os.path.splitext(baseName)[0] + ".png"
            surfaceDir = os.path.dirname(surfacePath)
            fullImageName = os.path.join(surfaceDir, imageName)            
                    
            # 导出图片
            if self.exportImage(fullImageName) == False:
                self.labelInfo.Text = "导出图片失败:" + fullImageName
                return False
                
        return True
    
    #     
    def computeVolume(self, selectedSim, material, volumeList = []):
        if len(volumeList) == 0:
            return False
         
        if self.checkbox_pause.Checked and self.stop_sim == True:
            return False
        
        # 设置 material 参数
        material.VOPType = SpeosSim.Material.EnumVOPType.Library
        material.SOPType = SpeosSim.Material.EnumSOPType.OpticalPolished
        
        for volumePath in volumeList:
            self.labelInfo.Text = "正在使用：" + volumePath
            material.VOPLibrary = volumePath
            
            if not self.compute_and_check(selectedSim):
                # 终止仿真
                self.stop_sim = True
                break
            
            #SpeosSim.Command.ComputeOnActiveSelection()
            
            baseName = os.path.basename(volumePath)
            imageName = os.path.splitext(baseName)[0] + ".png"
            volumeDir = os.path.dirname(volumePath)
            fullImageName = os.path.join(volumeDir, imageName)
            
            # 导出图片
            if self.exportImage(fullImageName) == False:
                self.labelInfo.Text = "导出图片失败:" + fullImageName
                return False
        return True

    def compute_and_check(self, selectedSim):
        # 执行仿真前，先删除目录中的xmp文件（若存在）
        if self.checkbox_pause.Checked:
            try:
                if Directory.Exists(self.outPath):
                    files = Directory.GetFiles(self.outPath)
                    for file_path in files:
                        try:
                            File.Delete(file_path)
                        except Exception as e:
                            print("无法删除文件 {0}: {1}".format(str(file_path), str(e)))
                else:
                    print("目录 {0} 不存在".format(str(self.outPath)))
            except Exception as e:
                self.handleError("删除xmp文件错误", e)

        # 执行 仿真
        if self.radio_btn_cpu.Checked:
            SpeosSim.Command.Compute(selectedSim.Items)
        else:
            SpeosSim.Command.GpuCompute(selectedSim.Items)
        print("计算结束")

        # 执行仿真后，若目标目录中不存在xmp文件
        # 如果勾选 出现错误，自动中止
        if self.checkbox_pause.Checked:
            if Directory.Exists(self.outPath):
                files = Directory.GetFiles(self.outPath, "*.xmp")
                if not files:
                    print("在目录 {0} 中没有找到以 .xmp 为后缀的文件。".format(str(self.outPath)))
                    return False
            else:
                print("目录 {0} 不存在".format(str(self.outPath)))
                return False

        return True

    def exportImage(self, imageName):
        theFile =""
        for file in os.listdir(self.outPath):
            if file.endswith(".xmp"):
                theFile = os.path.join(self.outPath, file)
                break
        if theFile == "":
            print("没有找到 xmp 文件，请检查路径是否正确。 '" + theFile + "'")
            #MessageBox.Show("没有找到 xmp 文件，请检查路径是否正确。", "错误", MessageBoxButtons.OK)
            return False
        
        self.labelInfo.Text = "正在导出：" + imageName
        print("导出图片: '" + imageName + "'")
        
        # 使用Virtual Huamn Vision Lab打开生成的xmp
        try:
            if self.XmpViewer.OpenFile(theFile) != 1:
                print("XmpViewer 打开文件失败！" )
                return False
        except Exception as e:
            self.handleError("在使用Virtual Huamn Vision Lab打开文件时出现异常", e)
        
        # 第一个参数为 保存的图片的文件名
        # 第二个参数为 图形格式(0: BMP, 1: PNG, 2 :TIFF, 3: JPG)
        if self.XmpViewer.ExportXMPImage(imageName, 1) == 0:
            print("导出图片失败! '" + imageName + "'")
            #MessageBox.Show("导出图片失败", "错误", MessageBoxButtons.OK)
            return False
        print("导出图片成功!")
        return True
    
    # 错误处理
    def handleError(self, message, exception):
        error_message = "{0}\n\n错误详情：{1}".format(message, str(exception))
        MessageBox.Show(error_message, "错误", MessageBoxButtons.OK)
        self.labelInfo.Text = error_message
        # 记录日志
        log_file_path = self.errorlogFile
        # 写入日志文件
        with open(log_file_path, "w") as f:
            f.write("{0}\n{1}\n\n".format(message, traceback.format_exc()))

    
def getFiles(path, suffixs = []):
    fileList = []
    
    if len(suffixs) == 0:
        for root, dirs, files in os.walk(path):
            for file in files:
                fileList.append(os.path.join(root, file))
    else:
        for root, dirs, files in os.walk(path):
            for file in files:
                suffix = os.path.splitext(file)[1].lower()
                
                if suffix not in suffixs: 
                    continue
                
                fileList.append(os.path.join(root, file))

    return fileList


def main():
    try:
        app = MainForm()
        
        # 非模态
        app.Show()
        # 模态
        #app.ShowDialog()
    except Exception as e:
        print(e.ToString())

main()
