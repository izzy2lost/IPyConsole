using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace Core;

public class Printer
{
    /// <summary>
    /// Print a specific range of pages within an XPS document.
    /// </summary>
    /// <param name="xpsFilePath">Path to source XPS file</param>
    /// <returns>Whether the document printed</returns>
    public static bool PrintDocumentPageRange(string xpsFilePath)
    {
        // Create the print dialog object and set options.
        PrintDialog printDialog = new()
        {
            UserPageRangeEnabled = true
        };

        // Display the dialog. This returns true if the user presses the Print button.
        bool? isPrinted = printDialog.ShowDialog();
        if (isPrinted != true)
            return false;

        // Print a specific page range within the document.
        try
        {
            // Open the selected document.
            XpsDocument xpsDocument = new(xpsFilePath, FileAccess.Read);

            // Get a fixed document sequence for the selected document.
            FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();

            // Create a paginator for all pages in the selected document.
            DocumentPaginator docPaginator = fixedDocSeq.DocumentPaginator;

            // Check whether a page range was specified in the print dialog.
            if (printDialog.PageRangeSelection == PageRangeSelection.UserPages)
            {
                // Create a document paginator for the specified range of pages.
                docPaginator = new DocPaginator(fixedDocSeq.DocumentPaginator, printDialog.PageRange);
            }

            // Print to a new file.
            printDialog.PrintDocument(docPaginator, $"Printing {Path.GetFileName(xpsFilePath)}");

            return true;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);

            return false;
        }
    }
    
    /// <summary>
    /// Returns a print ticket, which is a set of instructions telling a printer how
    /// to set its various features, such as duplexing, collating, and stapling.
    /// </summary>
    /// <param name="printQueue">The print queue to print to.</param>
    /// <returns>A print ticket.</returns>
    public static PrintTicket GetPrintTicket(PrintQueue printQueue)
    {
        PrintCapabilities printCapabilites = printQueue.GetPrintCapabilities();

        // Get a default print ticket from printer.
        PrintTicket printTicket = printQueue.DefaultPrintTicket;

        // Modify the print ticket.
        if (printCapabilites.CollationCapability.Contains(Collation.Collated))
            printTicket.Collation = Collation.Collated;
        if (printCapabilites.DuplexingCapability.Contains(Duplexing.TwoSidedLongEdge))
            printTicket.Duplexing = Duplexing.TwoSidedLongEdge;
        if (printCapabilites.StaplingCapability.Contains(Stapling.StapleDualLeft))
            printTicket.Stapling = Stapling.StapleDualLeft;

        // Returns a print ticket, which is a set of instructions telling a printer how
        // to set its various features, such as duplexing, collating, and stapling.
        return printTicket;
    }
    
    /// <summary>
    /// Return a collection of print queues, which individually hold the features or states
    /// of a printer as well as common properties for all print queues.
    /// </summary>
    /// <returns>A collection of print queues.</returns>
    public static PrintQueueCollection GetPrintQueues()
    {
        // Create a LocalPrintServer instance, which represents 
        // the print server for the local computer.
        LocalPrintServer localPrintServer = new();

        // Get the default print queue on the local computer.
        //PrintQueue printQueue = localPrintServer.DefaultPrintQueue;

        // Get all print queues on the local computer.
        PrintQueueCollection printQueueCollection = localPrintServer.GetPrintQueues();

        // Return a collection of print queues, which individually hold the features or states
        // of a printer as well as common properties for all print queues.
        return printQueueCollection;
    }
    
    /// <summary>
    /// Asynchronously, add the XPS document together with a print ticket to the print queue.
    /// </summary>
    /// <param name="xpsFilePath">Path to source XPS file.</param>
    /// <param name="printQueue">The print queue to print to.</param>
    /// <param name="printTicket">The print ticket for the selected print queue.</param>
    public static void PrintXpsDocumentAsync(string xpsFilePath, PrintQueue printQueue, PrintTicket printTicket)
    {
        // Create an XpsDocumentWriter object for the print queue.
        XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);

        // Open the selected document.
        XpsDocument xpsDocument = new(xpsFilePath, FileAccess.Read);

        // Get a fixed document sequence for the selected document.
        FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();

        // Asynchronously, add the XPS document together with a print ticket to the print queue.
        xpsDocumentWriter.WriteAsync(fixedDocSeq, printTicket);
    }

    /// <summary>
    /// Synchronously, add the XPS document together with a print ticket to the print queue.
    /// </summary>
    /// <param name="xpsFilePath">Path to source XPS file.</param>
    /// <param name="printQueue">The print queue to print to.</param>
    /// <param name="printTicket">The print ticket for the selected print queue.</param>
    public static void PrintXpsDocument(string xpsFilePath, PrintQueue printQueue, PrintTicket printTicket)
    {
        // Create an XpsDocumentWriter object for the print queue.
        XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);

        // Open the selected document.
        XpsDocument xpsDocument = new(xpsFilePath, FileAccess.Read);

        // Get a fixed document sequence for the selected document.
        FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();

        // Synchronously, add the XPS document together with a print ticket to the print queue.
        xpsDocumentWriter.Write(fixedDocSeq, printTicket);
    }
    /// <summary>
    /// Asyncronously, add a batch of XPS documents to the print queue using a PrintQueue.AddJob method.
    /// Handle the thread apartment state required by the PrintQueue.AddJob method.
    /// </summary>
    /// <param name="xpsFilePaths">A collection of XPS documents.</param>
    /// <param name="fastCopy">Whether to validate the XPS documents.</param>
    /// <returns>Whether all documents were added to the print queue.</returns>
    public static async Task<bool> BatchAddToPrintQueueAsync(IEnumerable<string> xpsFilePaths, bool fastCopy = false)
    {
        bool allAdded = true;

        // Queue some work to run on the ThreadPool.
        // Wait for completion without blocking the calling thread.
        await Task.Run(() =>
        {
            if (fastCopy)
                allAdded = BatchAddToPrintQueue(xpsFilePaths, fastCopy);
            else
            {
                // Create a thread to call the PrintQueue.AddJob method.
                Thread newThread = new(() =>
                {
                    allAdded = BatchAddToPrintQueue(xpsFilePaths, fastCopy);
                });

                // Set the thread to single-threaded apartment state.
                newThread.SetApartmentState(ApartmentState.STA);

                // Start the thread.
                newThread.Start();

                // Wait for thread completion. Blocks the calling thread,
                // which is a ThreadPool thread.
                newThread.Join();
            }
        });

        return allAdded;
    }

    /// <summary>
    /// Add a batch of XPS documents to the print queue using a PrintQueue.AddJob method.
    /// </summary>
    /// <param name="xpsFilePaths">A collection of XPS documents.</param>
    /// <param name="fastCopy">Whether to validate the XPS documents.</param>
    /// <returns>Whether all documents were added to the print queue.</returns>
    public static bool BatchAddToPrintQueue(IEnumerable<string> xpsFilePaths, bool fastCopy)
    {
        bool allAdded = true;

        // To print without getting the "Save Output File As" dialog, ensure
        // that your default printer is not the Microsoft XPS Document Writer,
        // Microsoft Print to PDF, or other print-to-file option.

        // Get a reference to the default print queue.
        PrintQueue defaultPrintQueue = LocalPrintServer.GetDefaultPrintQueue();

        // Iterate through the document collection.
        foreach (string xpsFilePath in xpsFilePaths)
        {
            // Get document name.
            string xpsFileName = Path.GetFileName(xpsFilePath);

            try
            {
                // The AddJob method adds a new print job for an XPS
                // document into the print queue, and assigns a job name.
                // Use fastCopy to skip XPS validation and progress notifications.
                // If fastCopy is false, the thread that calls PrintQueue.AddJob
                // must have a single-threaded apartment state.
                PrintSystemJobInfo xpsPrintJob =
                        defaultPrintQueue.AddJob(jobName: xpsFileName, documentPath: xpsFilePath, fastCopy);

                // If the queue is not paused and the printer is working, then jobs will automatically begin printing.
                Debug.WriteLine($"Added {xpsFileName} to the print queue.");
            }
            catch (PrintJobException e)
            {
                allAdded = false;
                Debug.WriteLine($"Failed to add {xpsFileName} to the print queue: {e.Message}\r\n{e.InnerException}");
            }
        }

        return allAdded;
    }
}

/// <summary>
/// Extend the abstract DocumentPaginator class to support page range printing. This class is based on the following online resources:
///
/// https://www.thomasclaudiushuber.com/2009/11/24/wpf-printing-how-to-print-a-pagerange-with-wpfs-printdialog-that-means-the-user-can-select-specific-pages-and-only-these-pages-are-printed/
///
/// https://social.msdn.microsoft.com/Forums/vstudio/en-US/9180e260-0791-4f2d-962d-abcb22ba8d09/how-to-print-multiple-page-ranges-with-wpf-printdialog
///
/// https://social.msdn.microsoft.com/Forums/en-US/841e804b-9130-4476-8709-0d2854c11582/exception-quotfixedpage-cannot-contain-another-fixedpagequot-when-printing-to-the-xps-document?forum=wpf
/// </summary>
public class DocPaginator : DocumentPaginator
{
    private readonly DocumentPaginator _documentPaginator;
    private readonly int _startPageIndex;
    private readonly int _endPageIndex;
    private readonly int _pageCount;

    public DocPaginator(DocumentPaginator documentPaginator, PageRange pageRange)
    {
        // Set document paginator.
        _documentPaginator = documentPaginator;

        // Set page indices.
        _startPageIndex = pageRange.PageFrom - 1;
        _endPageIndex = pageRange.PageTo - 1;

        // Validate and set page count.
        if (_startPageIndex >= 0 &&
            _endPageIndex >= 0 &&
            _startPageIndex <= _documentPaginator.PageCount - 1 &&
            _endPageIndex <= _documentPaginator.PageCount - 1 &&
            _startPageIndex <= _endPageIndex)
            _pageCount = _endPageIndex - _startPageIndex + 1;
    }

    public override bool IsPageCountValid => true;

    public override int PageCount => _pageCount;

    public override IDocumentPaginatorSource Source => _documentPaginator.Source;

    public override Size PageSize { get => _documentPaginator.PageSize; set => _documentPaginator.PageSize = value; }

    public override DocumentPage GetPage(int pageNumber)
    {
        DocumentPage documentPage = _documentPaginator.GetPage(_startPageIndex + pageNumber);

        // Workaround for "FixedPageInPage" exception.
        if (documentPage.Visual is FixedPage fixedPage)
        {
            var containerVisual = new ContainerVisual();
            foreach (object child in fixedPage.Children)
            {
                var childClone = (UIElement)child.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(child, null);

                FieldInfo parentField = childClone.GetType().GetField("_parent", BindingFlags.Instance | BindingFlags.NonPublic);
                if (parentField != null)
                {
                    parentField.SetValue(childClone, null);
                    containerVisual.Children.Add(childClone);
                }
            }

            return new DocumentPage(containerVisual, documentPage.Size, documentPage.BleedBox, documentPage.ContentBox);
        }

        return documentPage;
    }
}