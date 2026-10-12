using System.Drawing;
using System.Windows.Forms;

namespace Agenda
{
    // Caixa de entrada simples (equivalente ao InputBox do VB).
    public static class Dialogos
    {
        public static string Entrada(string titulo, string mensagem)
        {
            using (Form f = new Form())
            {
                f.Text = titulo;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.StartPosition = FormStartPosition.CenterParent;
                f.MaximizeBox = false;
                f.MinimizeBox = false;
                f.ClientSize = new Size(360, 120);

                Label lbl = new Label { Text = mensagem, Left = 12, Top = 12, Width = 336 };
                TextBox txt = new TextBox { Left = 12, Top = 40, Width = 336 };
                Button ok = new Button
                {
                    Text = "OK",
                    Left = 192,
                    Top = 78,
                    Width = 75,
                    DialogResult = DialogResult.OK
                };
                Button cancelar = new Button
                {
                    Text = "Cancelar",
                    Left = 273,
                    Top = 78,
                    Width = 75,
                    DialogResult = DialogResult.Cancel
                };

                f.Controls.AddRange(new Control[] { lbl, txt, ok, cancelar });
                f.AcceptButton = ok;
                f.CancelButton = cancelar;

                return f.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : null;
            }
        }
    }
}
