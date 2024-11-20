# -*- coding: utf-8 -*-
import clr

clr.AddReference('PresentationCore')
clr.AddReference('PresentationFramework')
clr.AddReference('WindowsBase')

from System.Windows import Application, Window
from System.Windows.Controls import Button


class DFWin(Window):
    def __init__(self):
        self.Title = "Console"
        self.Width = 300
        self.Height = 200

        self.btn = Button()
        self.btn.Width = 20
        self.btn.Height = 20
        self.btn.Content = "Click"


def main():
    app = Application()
    win = DFWin()
    app.Run(win)


if __name__ == "__main__":
    main()