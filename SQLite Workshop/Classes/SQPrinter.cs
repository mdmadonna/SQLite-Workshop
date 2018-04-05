using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class SQPrinter : PrintDocument
    {
        public Font PrinterFont { get; set; }
        public string TextToPrint { get; set; }
        static int curChar;

        public SQPrinter() : base()
        {
            //Set our Text property to an empty string
            TextToPrint = string.Empty;
        }

        public SQPrinter(string str) : base()
        {
            //Set our Text property value
            TextToPrint = str;
        }
        /// <summary>
        /// Override the default OnBeginPrint method of the PrintDocument Object
        /// </summary>
        /// <param name=e></param>
        /// <remarks></remarks>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            // Run base code
            base.OnBeginPrint(e);

            //Check to see if the user provided a font
            //if they didn't then we default to Times New Roman
            if (PrinterFont == null)
            {
                //Create the font we need
                PrinterFont = new Font("Courier New", 10);
            }
        }

        /// <summary>
        /// Override the default OnPrintPage method of the PrintDocument
        /// </summary>
        /// <param name=e></param>
        /// <remarks>This provides the print logic for our document</remarks>
        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            // Run base code
            base.OnPrintPage(e);

            //Declare local variables needed

            int printHeight;
            int printWidth;
            int leftMargin;
            int rightMargin;
            Int32 lines;
            Int32 chars;

            //Set print area size and margins
            {
                printHeight = base.DefaultPageSettings.PaperSize.Height - base.DefaultPageSettings.Margins.Top - base.DefaultPageSettings.Margins.Bottom;
                printWidth = base.DefaultPageSettings.PaperSize.Width - base.DefaultPageSettings.Margins.Left - base.DefaultPageSettings.Margins.Right;
                leftMargin = base.DefaultPageSettings.Margins.Left;  //X
                rightMargin = base.DefaultPageSettings.Margins.Top;  //Y
            }

            //Check if the user selected to print in Landscape mode
            //if they did then we need to swap height/width parameters
            if (base.DefaultPageSettings.Landscape)
            {
                int tmp;
                tmp = printHeight;
                printHeight = printWidth;
                printWidth = tmp;
            }

            //Create a rectangle printing are for our document
            RectangleF printArea = new RectangleF(leftMargin, rightMargin, printWidth, printHeight);

            //Use the StringFormat class for the text layout of our document
            StringFormat format = new StringFormat(StringFormatFlags.LineLimit);

            //Fit as many characters as we can into the print area
            e.Graphics.MeasureString(TextToPrint.Substring(RemoveZeros(ref curChar)), PrinterFont, new SizeF(printWidth, printHeight), format, out chars, out lines);

            //Print the page
            e.Graphics.DrawString(TextToPrint.Substring(RemoveZeros(ref curChar)), PrinterFont, Brushes.Black, printArea, format);

            //Increase current char count
            curChar += chars;

            //Detemine if there is more text to print, if
            //there is the tell the printer there is more coming
            if (curChar < TextToPrint.Length)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
                curChar = 0;
            }
        }

        public int RemoveZeros(ref int value)
        {
            //As 0 (ZERO) being sent to DrawString will create unexpected
            //problems when printing we need to search for the first
            //non-zero character in the string.
            while (TextToPrint[value] == 0)
            {
                value++;
            }
            return value;
        }

    }
}
