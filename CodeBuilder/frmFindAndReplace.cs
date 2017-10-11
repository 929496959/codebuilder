using CodeBuilder.Core;
// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CodeBuilder
{
    public partial class frmFindAndReplace : Form
    {
        public frmFindAndReplace()
        {
            InitializeComponent();
            search = new TextEditorSearcher();
        }

        TextEditorSearcher search;
        TextEditorControl editor;

        TextEditorControl Editor
        {
            get { return editor; }
            set
            {
                editor = value;
                search.Document = editor.Document;
                UpdateTitleBar();
            }
        }

        private void UpdateTitleBar()
        {
            Text = ReplaceMode ? "查找替换" : "查找";
        }

        public void ShowFor(TextEditorControl editor, bool replaceMode)
        {
            Editor = editor;

            search.ClearScanRegion();
            var sm = editor.ActiveTextAreaControl.SelectionManager;
            if (sm.HasSomethingSelected && sm.SelectionCollection.Count == 1)
            {
                var sel = sm.SelectionCollection[0];
                if (sel.StartPosition.Line == sel.EndPosition.Line)
                {
                    txtLookFor.Text = sm.SelectedText;
                }
                else
                {
                    search.SetScanRegion(sel);
                }
            }
            else
            {
                // Get the current word that the caret is on
                var caret = editor.ActiveTextAreaControl.Caret;
                var start = TextUtilities.FindWordStart(editor.Document, caret.Offset);
                var endAt = TextUtilities.FindWordEnd(editor.Document, caret.Offset);
                txtLookFor.Text = editor.Document.GetText(start, endAt - start);
            }

            ReplaceMode = replaceMode;

            Owner = (Form)editor.TopLevelControl;
            Show();

            txtLookFor.SelectAll();
            txtLookFor.Focus();
        }

        public bool ReplaceMode
        {
            get { return txtReplaceWith.Visible; }
            set
            {
                btnReplace.Visible = btnReplaceAll.Visible = value;
                lblReplaceWith.Visible = txtReplaceWith.Visible = value;
                btnHighlightAll.Visible = !value;
                AcceptButton = value ? btnReplace : btnFindNext;
                if (value)
                {
                    btnCancel.Top = 124;
                    chkMatchCase.Top = chkMatchWholeWord.Top = 69;
                    Height = 205;
                }
                else
                {
                    btnCancel.Top = 97;
                    chkMatchCase.Top = chkMatchWholeWord.Top = 50;
                    Height = 177;
                }

                UpdateTitleBar();
            }
        }

        private void btnFindPrevious_Click(object sender, EventArgs e)
        {
            FindNext(false, true, "文本没有找到。");
        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            FindNext(false, false, "文本没有找到。");
        }

        public bool _lastSearchWasBackward = false;
        public bool _lastSearchLoopedAround;

        public TextRange FindNext(bool viaF3, bool searchBackward, string messageIfNotFound)
        {
            if (string.IsNullOrEmpty(txtLookFor.Text))
            {
                return null;
            }


            _lastSearchWasBackward = searchBackward;
            search.LookFor = txtLookFor.Text;
            search.MatchCase = chkMatchCase.Checked;
            search.MatchWholeWordOnly = chkMatchWholeWord.Checked;

            var caret = editor.ActiveTextAreaControl.Caret;
            if (viaF3 && search.HasScanRegion && !caret.Offset.
                IsInRange(search.BeginOffset, search.EndOffset))
            {
                // user moved outside of the originally selected region
                search.ClearScanRegion();
                UpdateTitleBar();
            }

            var startFrom = caret.Offset - (searchBackward ? 1 : 0);
            var range = search.FindNext(startFrom, searchBackward, out _lastSearchLoopedAround);
            if (range != null)
            {
                SelectResult(range);
            }
            else if (messageIfNotFound != null)
            {
                MessageBoxHelper.ShowExclamation(messageIfNotFound);
            }

            return range;
        }

        private void SelectResult(TextRange range)
        {
            var p1 = editor.Document.OffsetToPosition(range.Offset);
            var p2 = editor.Document.OffsetToPosition(range.Offset + range.Length);
            editor.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2);
            editor.ActiveTextAreaControl.ScrollTo(p1.Line, p1.Column);
            // Also move the caret to the end of the selection, because when the user 
            // presses F3, the caret is where we start searching next time.
            editor.ActiveTextAreaControl.Caret.Position =
                editor.Document.OffsetToPosition(range.Offset + range.Length);
        }

        Dictionary<TextEditorControl, HighlightGroup> highlightGroups = new Dictionary<TextEditorControl, HighlightGroup>();

        private void btnHighlightAll_Click(object sender, EventArgs e)
        {
            if (!highlightGroups.ContainsKey(editor))
            {
                highlightGroups[editor] = new HighlightGroup(editor);
            }

            var group = highlightGroups[editor];

            if (string.IsNullOrEmpty(LookFor))
            {
                // Clear highlights
                group.ClearMarkers();
            }
            else
            {
                search.LookFor = txtLookFor.Text;
                search.MatchCase = chkMatchCase.Checked;
                search.MatchWholeWordOnly = chkMatchWholeWord.Checked;

                var looped = false;
                int offset = 0, count = 0;
                for (; ; )
                {
                    TextRange range = search.FindNext(offset, false, out looped);
                    if (range == null || looped)
                        break;
                    offset = range.Offset + range.Length;
                    count++;

                    var m = new TextMarker(range.Offset, range.Length,
                            TextMarkerType.SolidBlock, Color.Yellow, Color.Black);
                    group.AddMarker(m);
                }

                if (count == 0)
                {
                    MessageBoxHelper.ShowExclamation("文本没有找到。");
                }
                else
                {
                    Close();
                }
            }
        }

        private void FindAndReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent dispose, as this form can be re-used
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                if (this.Owner != null)
                {
                    this.Owner.Select(); // prevent another app from being activated instead
                }

                e.Cancel = true;
                Hide();

                // Discard search region
                search.ClearScanRegion();
                editor.Refresh(); // must repaint manually
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            var sm = editor.ActiveTextAreaControl.SelectionManager;
            if (string.Equals(sm.SelectedText, txtLookFor.Text, StringComparison.OrdinalIgnoreCase))
            {
                InsertText(txtReplaceWith.Text);
            }

            FindNext(false, _lastSearchWasBackward, "文本没有找到。");
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            int count = 0;
            // BUG FIX: if the replacement string contains the original search string
            // (e.g. replace "red" with "very red") we must avoid looping around and
            // replacing forever! To fix, start replacing at beginning of region (by 
            // moving the caret) and stop as soon as we loop around.
            editor.ActiveTextAreaControl.Caret.Position =
                editor.Document.OffsetToPosition(search.BeginOffset);

            editor.Document.UndoStack.StartUndoGroup();
            try
            {
                while (FindNext(false, false, null) != null)
                {
                    if (_lastSearchLoopedAround)
                        break;

                    // Replace
                    count++;
                    InsertText(txtReplaceWith.Text);
                }
            }
            finally
            {
                editor.Document.UndoStack.EndUndoGroup();
            }

            if (count == 0)
            {
                MessageBoxHelper.ShowExclamation("没有单词被替换。");
            }
            else
            {
                MessageBoxHelper.ShowInformation(string.Format("替换了 {0} 处单词。", count));
                Close();
            }
        }

        private void InsertText(string text)
        {
            var textArea = editor.ActiveTextAreaControl.TextArea;
            textArea.Document.UndoStack.StartUndoGroup();
            try
            {
                if (textArea.SelectionManager.HasSomethingSelected)
                {
                    textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
                    textArea.SelectionManager.RemoveSelectedText();
                }
                textArea.InsertString(text);
            }
            finally
            {
                textArea.Document.UndoStack.EndUndoGroup();
            }
        }

        public string LookFor { get { return txtLookFor.Text; } }
    }

    public class TextRange : AbstractSegment
    {
        IDocument _document;
        public TextRange(IDocument document, int offset, int length)
        {
            _document = document;
            this.offset = offset;
            this.length = length;
        }
    }

    /// <summary>This class finds occurrances of a search string in a text 
    /// editor's IDocument... it's like Find box without a GUI.</summary>
    public class TextEditorSearcher : IDisposable
    {
        IDocument _document;
        public IDocument Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    ClearScanRegion();
                    _document = value;
                }
            }
        }

        // I would have used the TextAnchor class to represent the beginning and 
        // end of the region to scan while automatically adjusting to changes in 
        // the document--but for some reason it is sealed and its constructor is 
        // internal. Instead I use a TextMarker, which is perhaps even better as 
        // it gives me the opportunity to highlight the region. Note that all the 
        // markers and coloring information is associated with the text document, 
        // not the editor control, so TextEditorSearcher doesn't need a reference 
        // to the TextEditorControl. After adding the marker to the document, we
        // must remember to remove it when it is no longer needed.
        TextMarker _region = null;
        /// <summary>Sets the region to search. The region is updated 
        /// automatically as the document changes.</summary>
        public void SetScanRegion(ISelection sel)
        {
            SetScanRegion(sel.Offset, sel.Length);
        }
        /// <summary>Sets the region to search. The region is updated 
        /// automatically as the document changes.</summary>
        public void SetScanRegion(int offset, int length)
        {
            var bkgColor = _document.HighlightingStrategy.GetColorFor("Default").BackgroundColor;
            _region = new TextMarker(offset, length, TextMarkerType.SolidBlock,
                bkgColor.HalfMix(Color.FromArgb(160, 160, 160)));
            _document.MarkerStrategy.AddMarker(_region);
        }
        public bool HasScanRegion
        {
            get { return _region != null; }
        }
        public void ClearScanRegion()
        {
            if (_region != null)
            {
                _document.MarkerStrategy.RemoveMarker(_region);
                _region = null;
            }
        }
        public void Dispose() { ClearScanRegion(); GC.SuppressFinalize(this); }
        ~TextEditorSearcher() { Dispose(); }

        /// <summary>Begins the start offset for searching</summary>
        public int BeginOffset
        {
            get
            {
                if (_region != null)
                    return _region.Offset;
                else
                    return 0;
            }
        }
        /// <summary>Begins the end offset for searching</summary>
        public int EndOffset
        {
            get
            {
                if (_region != null)
                    return _region.EndOffset;
                else
                    return _document.TextLength;
            }
        }

        public bool MatchCase;

        public bool MatchWholeWordOnly;

        string _lookFor;
        string _lookFor2; // uppercase in case-insensitive mode
        public string LookFor
        {
            get { return _lookFor; }
            set { _lookFor = value; }
        }

        /// <summary>Finds next instance of LookFor, according to the search rules 
        /// (MatchCase, MatchWholeWordOnly).</summary>
        /// <param name="beginAtOffset">Offset in Document at which to begin the search</param>
        /// <remarks>If there is a match at beginAtOffset precisely, it will be returned.</remarks>
        /// <returns>Region of document that matches the search string</returns>
        public TextRange FindNext(int beginAtOffset, bool searchBackward, out bool loopedAround)
        {
            loopedAround = false;

            int startAt = BeginOffset, endAt = EndOffset;
            int curOffs = beginAtOffset.InRange(startAt, endAt);

            _lookFor2 = MatchCase ? _lookFor : _lookFor.ToUpperInvariant();

            TextRange result;
            if (searchBackward)
            {
                result = FindNextIn(startAt, curOffs, true);
                if (result == null)
                {
                    loopedAround = true;
                    result = FindNextIn(curOffs, endAt, true);
                }
            }
            else
            {
                result = FindNextIn(curOffs, endAt, false);
                if (result == null)
                {
                    loopedAround = true;
                    result = FindNextIn(startAt, curOffs, false);
                }
            }
            return result;
        }

        private TextRange FindNextIn(int offset1, int offset2, bool searchBackward)
        {
            offset2 -= _lookFor.Length;

            // Make behavior decisions before starting search loop
            Func<char, char, bool> matchFirstCh;
            Func<int, bool> matchWord;
            if (MatchCase)
                matchFirstCh = (lookFor, c) => (lookFor == c);
            else
                matchFirstCh = (lookFor, c) => (lookFor == Char.ToUpperInvariant(c));
            if (MatchWholeWordOnly)
                matchWord = IsWholeWordMatch;
            else
                matchWord = IsPartWordMatch;

            // Search
            char lookForCh = _lookFor2[0];
            if (searchBackward)
            {
                for (int offset = offset2; offset >= offset1; offset--)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset))
                        && matchWord(offset))
                        return new TextRange(_document, offset, _lookFor.Length);
                }
            }
            else
            {
                for (int offset = offset1; offset <= offset2; offset++)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset))
                        && matchWord(offset))
                        return new TextRange(_document, offset, _lookFor.Length);
                }
            }
            return null;
        }
        private bool IsWholeWordMatch(int offset)
        {
            if (IsWordBoundary(offset) && IsWordBoundary(offset + _lookFor.Length))
                return IsPartWordMatch(offset);
            else
                return false;
        }
        private bool IsWordBoundary(int offset)
        {
            return offset <= 0 || offset >= _document.TextLength ||
                !IsAlphaNumeric(offset - 1) || !IsAlphaNumeric(offset);
        }
        private bool IsAlphaNumeric(int offset)
        {
            char c = _document.GetCharAt(offset);
            return Char.IsLetterOrDigit(c) || c == '_';
        }
        private bool IsPartWordMatch(int offset)
        {
            string substr = _document.GetText(offset, _lookFor.Length);
            if (!MatchCase)
                substr = substr.ToUpperInvariant();
            return substr == _lookFor2;
        }
    }

    /// <summary>Bundles a group of markers together so that they can be cleared 
    /// together.</summary>
    public class HighlightGroup : IDisposable
    {
        List<TextMarker> markers = new List<TextMarker>();
        TextEditorControl editor;
        IDocument document;

        public HighlightGroup(TextEditorControl editor)
        {
            this.editor = editor;
            document = editor.Document;
        }

        public void AddMarker(TextMarker marker)
        {
            markers.Add(marker);
            document.MarkerStrategy.AddMarker(marker);
        }

        public void ClearMarkers()
        {
            foreach (TextMarker m in markers)
                document.MarkerStrategy.RemoveMarker(m);
            markers.Clear();
            editor.Refresh();
        }

        public void Dispose() { ClearMarkers(); GC.SuppressFinalize(this); }

        ~HighlightGroup() { Dispose(); }

        public IList<TextMarker> Markers { get { return markers.AsReadOnly(); } }
    }

    internal static class Extensions
    {
        public static int InRange(this int x, int lo, int hi)
        {
            return x < lo ? lo : (x > hi ? hi : x);
        }
        public static bool IsInRange(this int x, int lo, int hi)
        {
            return x >= lo && x <= hi;
        }
        public static Color HalfMix(this Color one, Color two)
        {
            return Color.FromArgb(
                (one.A + two.A) >> 1,
                (one.R + two.R) >> 1,
                (one.G + two.G) >> 1,
                (one.B + two.B) >> 1);
        }
    }
}
