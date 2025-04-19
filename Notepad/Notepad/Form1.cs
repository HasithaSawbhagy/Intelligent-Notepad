using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; // Required for file operations

// Ensure this namespace matches your project's namespace
namespace Notepad
{
    public partial class Form1 : Form
    {
        // Member variables
        private string currentFilePath = null; // Holds the path of the currently open file
        private bool isTextChanged = false;    // Flag to track unsaved changes

        // Constructor
        public Form1()
        {
            InitializeComponent();
            InitializeMyComponents(); // Call custom initialization
        }

        // Custom initialization logic
        private void InitializeMyComponents()
        {
            // Set initial state for Word Wrap menu item
            wordWrapToolStripMenuItem.Checked = mainTextBox.WordWrap;

            // Set the initial window title
            UpdateWindowTitle();

            // Attach event handlers programmatically
            // Alternatively, ensure these are connected in the Form Designer's Events section
            this.mainTextBox.TextChanged += new System.EventHandler(this.mainTextBox_TextChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);

            // Configure dialogs (optional, but good practice)
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.Title = "Open Text File";
            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog1.Title = "Save Text File";
        }


        // --- Helper Methods ---

        /// <summary>
        /// Updates the window title based on the current file path and modification state.
        /// </summary>
        private void UpdateWindowTitle()
        {
            string fileName = string.IsNullOrEmpty(currentFilePath) ? "Untitled" : Path.GetFileName(currentFilePath);
            string modifiedMarker = isTextChanged ? "*" : ""; // Add asterisk if modified
            this.Text = $"{modifiedMarker}{fileName} - Notepad"; // Use the original "Notepad" branding
        }

        /// <summary>
        /// Prompts the user to save changes if the text has been modified.
        /// </summary>
        /// <returns>DialogResult.Cancel if the user cancels the operation, otherwise DialogResult.OK/Yes/No.</returns>
        private DialogResult CheckUnsavedChanges()
        {
            if (!isTextChanged)
            {
                return DialogResult.OK; // No changes, proceed
            }

            string message = $"Do you want to save changes to {(string.IsNullOrEmpty(currentFilePath) ? "Untitled" : Path.GetFileName(currentFilePath))}?";
            DialogResult result = MessageBox.Show(message, "Notepad", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Try to save; if save is successful or user cancels save dialog, return appropriate result
                return SaveFile(); // SaveFile will return Cancel if user cancels the Save As dialog
            }
            else if (result == DialogResult.No)
            {
                return DialogResult.No; // User chose not to save, proceed with original action
            }
            else // Result is DialogResult.Cancel
            {
                return DialogResult.Cancel; // User cancelled the action, stop original action
            }
        }

        /// <summary>
        /// Saves the current content to a file. Handles both Save and Save As logic.
        /// </summary>
        /// <returns>DialogResult.Cancel if the user cancels the Save As dialog, otherwise DialogResult.OK.</returns>
        private DialogResult SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                // No current file path, use Save As logic
                saveFileDialog1.FileName = "Untitled.txt"; // Suggest a default name
                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    currentFilePath = saveFileDialog1.FileName;
                }
                else
                {
                    return DialogResult.Cancel; // User cancelled the Save As dialog
                }
            }

            // Proceed with saving
            try
            {
                File.WriteAllText(currentFilePath, mainTextBox.Text);
                isTextChanged = false; // Mark changes as saved
                UpdateWindowTitle();
                return DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return DialogResult.Abort; // Indicate save failed, but not necessarily a user cancel
            }
        }


        // --- Event Handlers ---

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This handler is usually for the top-level "File" menu itself,
            // often left empty unless you want specific behavior when clicking "File".
        }

        // --- File Menu Event Handlers ---

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = CheckUnsavedChanges();
            if (result != DialogResult.Cancel) // Proceed if user didn't cancel
            {
                mainTextBox.Clear();
                currentFilePath = null;
                isTextChanged = false; // Reset flag
                UpdateWindowTitle();   // Update title for "Untitled"
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult checkResult = CheckUnsavedChanges();
            if (checkResult == DialogResult.Cancel)
            {
                return; // User cancelled
            }

            // Reuse existing dialog settings set in InitializeMyComponents
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    currentFilePath = openFileDialog1.FileName;
                    mainTextBox.Text = File.ReadAllText(currentFilePath);
                    isTextChanged = false; // Freshly loaded file has no unsaved changes
                    UpdateWindowTitle();   // Update title with new filename
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    currentFilePath = null; // Reset path on error
                    mainTextBox.Clear();    // Clear text area on error
                    isTextChanged = false;
                    UpdateWindowTitle();
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(); // Use the helper method for Save/SaveAs logic
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Force Save As logic by temporarily clearing the current path
            string previousPath = currentFilePath;         // Store old path
            bool previousTextChangedState = isTextChanged; // Store old state
            currentFilePath = null;                        // Force Save As dialog in SaveFile()

            DialogResult result = SaveFile(); // Use the helper method

            if (result == DialogResult.Cancel || result == DialogResult.Abort)
            {
                // If user cancelled Save As or it failed, restore previous state
                currentFilePath = previousPath;
                isTextChanged = previousTextChangedState; // Restore modification status
                UpdateWindowTitle(); // Update title back if needed (in case it was changed during failed Save As)
            }
            // If OK, SaveFile already updated path, state, and title
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Trigger the FormClosing event logic by calling Close()
            this.Close();
        }

        // --- Edit Menu Event Handlers ---

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainTextBox.SelectionLength > 0)
            {
                mainTextBox.Cut();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainTextBox.SelectionLength > 0)
            {
                mainTextBox.Copy();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if clipboard contains text data before pasting
            if (Clipboard.ContainsText())
            {
                mainTextBox.Paste();
            }
        }

        // --- Format Menu Event Handlers ---

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle word wrap and the menu item's checked state
            mainTextBox.WordWrap = !mainTextBox.WordWrap;
            wordWrapToolStripMenuItem.Checked = mainTextBox.WordWrap;
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = mainTextBox.Font; // Show current font initially
            if (fontDialog1.ShowDialog(this) == DialogResult.OK)
            {
                mainTextBox.Font = fontDialog1.Font;
            }
        }

        // --- Help Menu Event Handlers ---

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Updated version number/description slightly
            MessageBox.Show("Notepad v1.1\nA basic text editor created with C# WinForms.\nIncludes unsaved changes check.",
                            "About Notepad", // Keep original title
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        // --- TextBox Event Handler ---
        private void mainTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isTextChanged) // Only update title visually the first time text changes
            {
                isTextChanged = true;
                UpdateWindowTitle(); // Add the asterisk to the title
            }
        }

        // --- Form Event Handler ---
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check for unsaved changes when the form is about to close
            // This handles clicking the 'X' button as well as File->Exit
            DialogResult result = CheckUnsavedChanges();

            if (result == DialogResult.Cancel)
            {
                // User cancelled the closing operation, prevent the form from closing
                e.Cancel = true;
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}