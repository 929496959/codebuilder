﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Windows.Forms;

namespace CodeBuilder.Core
{
    public static class MessageBoxHelper
    {
        public static void ShowExclamation(string message)
        {
            StaticUnity.Main.Invoke(new Action(() =>
            {
                MessageBox.Show(StaticUnity.Main, message, "CodeBuilder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }));
        }

        public static void ShowError(string message)
        {
            StaticUnity.Main.Invoke(new Action(() =>
            {
                MessageBox.Show(StaticUnity.Main, message, "CodeBuilder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        public static void ShowInformation(string message)
        {
            StaticUnity.Main.Invoke(new Action(() =>
            {
                MessageBox.Show(StaticUnity.Main, message, "CodeBuilder", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        public static DialogResult ShowQuestion(string message)
        {
            return MessageBox.Show(StaticUnity.Main, message, "CodeBuilder", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static DialogResult ShowQuestionEx(string message)
        {
            return MessageBox.Show(StaticUnity.Main, message, "CodeBuilder", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
    }
}
