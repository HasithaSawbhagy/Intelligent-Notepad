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

namespace Notepad
{
    public partial class Form1 : Form
    {
        private string currentFilePath = null; // Holds the path of the currently open file
        public Form1()
        {
            InitializeComponent();
            // Set initial state for Word Wrap menu item
            wordWrapToolStripMenuItem.Checked = mainTextBox.WordWrap;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // --- File Menu ---

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Add check for unsaved changes before clearing
            mainTextBox.Clear();
            currentFilePath = null;
            this.Text = "Untitled - Notepad"; // Update window title
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Add check for unsaved changes before opening
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"; // Filter for .txt files
            openFileDialog1.Title = "Open Text File";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    currentFilePath = openFileDialog1.FileName;
                    mainTextBox.Text = File.ReadAllText(currentFilePath);
                    this.Text = Path.GetFileName(currentFilePath) + " - Notepad"; // Update window title
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    currentFilePath = null; // Reset path on error
                    this.Text = "MyNotepadClone";
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                // If no file path exists, behave like Save As
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                try
                {
                    File.WriteAllText(currentFilePath, mainTextBox.Text);
                    this.Text = Path.GetFileName(currentFilePath) + " - Notepad"; // Update window title
                                                                                         // TODO: Reset 'unsaved changes' flag here
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog1.Title = "Save Text File As";
            saveFileDialog1.FileName = "Untitled.txt"; // Default file name

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    currentFilePath = saveFileDialog1.FileName;
                    File.WriteAllText(currentFilePath, mainTextBox.Text);
                    this.Text = Path.GetFileName(currentFilePath) + " - Notepad"; // Update window title
                                                                                         // TODO: Reset 'unsaved changes' flag here
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    currentFilePath = null; // Reset path on error (might want to keep it to allow retry?)
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Add check for unsaved changes before exiting
            Application.Exit();
        }

        // --- Edit Menu ---

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
            mainTextBox.Paste();
        }

        // --- Format Menu ---

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle word wrap and the menu item's checked state
            mainTextBox.WordWrap = !mainTextBox.WordWrap;
            wordWrapToolStripMenuItem.Checked = mainTextBox.WordWrap;
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = mainTextBox.Font; // Show current font initially
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                mainTextBox.Font = fontDialog1.Font;
            }
        }

        // --- Help Menu ---

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Notepad v1.0\nA basic text editor created with C# WinForms.", "About Notepad", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
