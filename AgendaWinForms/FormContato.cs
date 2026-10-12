using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Agenda
{
    // Formulário usado para adicionar e para alterar um contato.
    public class FormContato : Form
    {
        private readonly TextBox txtEmail = new TextBox();
        private readonly TextBox txtNome = new TextBox();
        private readonly MaskedTextBox mskNasc = new MaskedTextBox();
        private readonly ComboBox cmbTipo = new ComboBox();
        private readonly TextBox txtNumero = new TextBox();
        private readonly CheckBox chkPrincipal = new CheckBox();
        private readonly ListView lvTel = new ListView();
        private readonly Button btnAddTel = new Button();
        private readonly Button btnRemTel = new Button();
        private readonly Button btnOk = new Button();
        private readonly Button btnCancelar = new Button();

        private readonly List<Telefone> telefones = new List<Telefone>();

        public Contato Resultado { get; private set; }

        public FormContato(Contato existente = null)
        {
            Text = existente == null ? "Adicionar contato" : "Alterar contato";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(460, 440);

            // --- Dados do contato ---
            Controls.Add(new Label { Text = "E-mail:", Left = 12, Top = 16, Width = 130 });
            txtEmail.SetBounds(150, 12, 298, 23);
            Controls.Add(txtEmail);

            Controls.Add(new Label { Text = "Nome:", Left = 12, Top = 48, Width = 130 });
            txtNome.SetBounds(150, 44, 298, 23);
            Controls.Add(txtNome);

            Controls.Add(new Label { Text = "Nascimento (dd/mm/aaaa):", Left = 12, Top = 80, Width = 140 });
            mskNasc.Mask = "00/00/0000";
            mskNasc.PromptChar = '_';
            mskNasc.SetBounds(150, 76, 110, 23);
            Controls.Add(mskNasc);

            // --- Telefones ---
            GroupBox grp = new GroupBox { Text = "Telefones", Left = 12, Top = 112, Width = 436, Height = 270 };

            grp.Controls.Add(new Label { Text = "Tipo:", Left = 12, Top = 26, Width = 40 });
            cmbTipo.DropDownStyle = ComboBoxStyle.DropDown;
            cmbTipo.Items.AddRange(new object[] { "Celular", "Residencial", "Comercial" });
            cmbTipo.SelectedIndex = 0;
            cmbTipo.SetBounds(56, 22, 120, 23);
            grp.Controls.Add(cmbTipo);

            grp.Controls.Add(new Label { Text = "Número:", Left = 190, Top = 26, Width = 55 });
            txtNumero.SetBounds(248, 22, 176, 23);
            grp.Controls.Add(txtNumero);

            chkPrincipal.Text = "Principal";
            chkPrincipal.SetBounds(56, 54, 100, 23);
            grp.Controls.Add(chkPrincipal);

            btnAddTel.Text = "Adicionar";
            btnAddTel.SetBounds(248, 52, 85, 26);
            btnAddTel.Click += BtnAddTel_Click;
            grp.Controls.Add(btnAddTel);

            btnRemTel.Text = "Remover";
            btnRemTel.SetBounds(339, 52, 85, 26);
            btnRemTel.Click += BtnRemTel_Click;
            grp.Controls.Add(btnRemTel);

            lvTel.View = View.Details;
            lvTel.FullRowSelect = true;
            lvTel.GridLines = true;
            lvTel.MultiSelect = false;
            lvTel.Columns.Add("Tipo", 110);
            lvTel.Columns.Add("Número", 200);
            lvTel.Columns.Add("Principal", 90);
            lvTel.SetBounds(12, 88, 412, 170);
            grp.Controls.Add(lvTel);

            Controls.Add(grp);

            // --- Botões ---
            btnOk.Text = "Salvar";
            btnOk.SetBounds(292, 396, 75, 28);
            btnOk.Click += BtnOk_Click;
            Controls.Add(btnOk);

            btnCancelar.Text = "Cancelar";
            btnCancelar.SetBounds(373, 396, 75, 28);
            btnCancelar.DialogResult = DialogResult.Cancel;
            Controls.Add(btnCancelar);

            AcceptButton = btnOk;
            CancelButton = btnCancelar;

            // --- Preenche os dados no modo de alteração ---
            if (existente != null)
            {
                txtEmail.Text = existente.Email;
                txtEmail.ReadOnly = true; // o e-mail identifica o contato
                txtNome.Text = existente.Nome;
                mskNasc.Text = existente.DtNasc.ToString().Replace("/", "");

                foreach (Telefone t in existente.Telefones)
                    telefones.Add(new Telefone(t.Tipo, t.Numero, t.Principal));

                AtualizarTelefones();
            }
        }

        private void AtualizarTelefones()
        {
            lvTel.Items.Clear();
            foreach (Telefone t in telefones)
            {
                ListViewItem item = new ListViewItem(t.Tipo);
                item.SubItems.Add(t.Numero);
                item.SubItems.Add(t.Principal ? "Sim" : "Não");
                lvTel.Items.Add(item);
            }
        }

        private void BtnAddTel_Click(object sender, EventArgs e)
        {
            string numero = txtNumero.Text.Trim();
            if (numero == "")
            {
                MessageBox.Show("Informe o número do telefone.", "Telefone",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNumero.Focus();
                return;
            }

            string tipo = string.IsNullOrWhiteSpace(cmbTipo.Text) ? "Celular" : cmbTipo.Text.Trim();
            bool principal = chkPrincipal.Checked || telefones.Count == 0; // o 1º telefone é o principal

            // Só pode haver um telefone principal
            if (principal)
            {
                foreach (Telefone t in telefones)
                    t.Principal = false;
            }

            telefones.Add(new Telefone(tipo, numero, principal));
            AtualizarTelefones();

            txtNumero.Clear();
            chkPrincipal.Checked = false;
            txtNumero.Focus();
        }

        private void BtnRemTel_Click(object sender, EventArgs e)
        {
            if (lvTel.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Selecione um telefone na lista.", "Telefone",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int i = lvTel.SelectedIndices[0];
            bool eraPrincipal = telefones[i].Principal;
            telefones.RemoveAt(i);

            // Se removeu o principal, o primeiro que sobrar assume
            if (eraPrincipal && telefones.Count > 0)
                telefones[0].Principal = true;

            AtualizarTelefones();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string nome = txtNome.Text.Trim();

            if (email == "" || !email.Contains("@"))
            {
                MessageBox.Show("Informe um e-mail válido.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (nome == "")
            {
                MessageBox.Show("Informe o nome.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return;
            }

            if (!mskNasc.MaskCompleted || !Data.TryParse(mskNasc.Text, out Data dtNasc))
            {
                MessageBox.Show("Informe uma data de nascimento válida (dd/mm/aaaa).", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                mskNasc.Focus();
                return;
            }

            Contato c = new Contato(email, nome, dtNasc);
            foreach (Telefone t in telefones)
                c.adicionarTelefone(new Telefone(t.Tipo, t.Numero, t.Principal));

            Resultado = c;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
