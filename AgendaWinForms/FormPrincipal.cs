using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Agenda
{
    public class FormPrincipal : Form
    {
        private readonly Contatos contatos = new Contatos();

        private readonly ListView lvContatos = new ListView();
        private readonly Label lblTotal = new Label();

        public FormPrincipal()
        {
            Text = "Agenda de Contatos";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(820, 480);
            MinimumSize = new Size(700, 350);

            // ---------- Menu (opções do seletor) ----------
            MenuStrip menu = new MenuStrip();
            ToolStripMenuItem mnuContatos = new ToolStripMenuItem("&Contatos");
            mnuContatos.DropDownItems.Add("&Adicionar contato", null, (s, e) => AdicionarContato());
            mnuContatos.DropDownItems.Add("&Pesquisar contato", null, (s, e) => PesquisarContato());
            mnuContatos.DropDownItems.Add("A&lterar contato", null, (s, e) => AlterarContato());
            mnuContatos.DropDownItems.Add("&Remover contato", null, (s, e) => RemoverContato());
            mnuContatos.DropDownItems.Add("L&istar contatos", null, (s, e) => ListarContatos());
            mnuContatos.DropDownItems.Add(new ToolStripSeparator());
            mnuContatos.DropDownItems.Add("&Sair", null, (s, e) => Close());
            menu.Items.Add(mnuContatos);
            MainMenuStrip = menu;

            // ---------- Barra de botões ----------
            FlowLayoutPanel barra = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(8, 8, 8, 4)
            };
            barra.Controls.Add(CriarBotao("Adicionar", AdicionarContato));
            barra.Controls.Add(CriarBotao("Pesquisar", PesquisarContato));
            barra.Controls.Add(CriarBotao("Alterar", AlterarContato));
            barra.Controls.Add(CriarBotao("Remover", RemoverContato));
            barra.Controls.Add(CriarBotao("Listar", ListarContatos));
            barra.Controls.Add(CriarBotao("Sair", Close));

            // ---------- Lista ----------
            lvContatos.Dock = DockStyle.Fill;
            lvContatos.View = View.Details;
            lvContatos.FullRowSelect = true;
            lvContatos.GridLines = true;
            lvContatos.MultiSelect = false;
            lvContatos.HideSelection = false;
            lvContatos.Columns.Add("Nome", 200);
            lvContatos.Columns.Add("E-mail", 220);
            lvContatos.Columns.Add("Nascimento", 90);
            lvContatos.Columns.Add("Idade", 50);
            lvContatos.Columns.Add("Telefone principal", 180);
            lvContatos.DoubleClick += (s, e) => AlterarContato();

            // ---------- Rodapé ----------
            lblTotal.Dock = DockStyle.Bottom;
            lblTotal.Height = 24;
            lblTotal.TextAlign = ContentAlignment.MiddleLeft;
            lblTotal.Padding = new Padding(8, 0, 0, 0);

            // A ordem de inclusão importa por causa do Dock
            Controls.Add(lvContatos);
            Controls.Add(lblTotal);
            Controls.Add(barra);
            Controls.Add(menu);

            ListarContatos();
        }

        private Button CriarBotao(string texto, Action acao)
        {
            Button b = new Button { Text = texto, Width = 100, Height = 28 };
            b.Click += (s, e) => acao();
            return b;
        }

        // ---------- Opções ----------

        private void AdicionarContato()
        {
            using (FormContato f = new FormContato())
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                if (contatos.adicionar(f.Resultado))
                {
                    ListarContatos();
                    SelecionarNaLista(f.Resultado);
                }
                else
                {
                    MessageBox.Show("Já existe um contato com esse e-mail.", "Adicionar contato",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void PesquisarContato()
        {
            string email = Dialogos.Entrada("Pesquisar contato", "Informe o e-mail do contato:");
            if (string.IsNullOrEmpty(email)) return;

            Contato encontrado = contatos.pesquisar(new Contato(email, "", null));

            if (encontrado == null)
            {
                MessageBox.Show("Contato não encontrado.", "Pesquisar contato",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ListarContatos();
            SelecionarNaLista(encontrado);
            MessageBox.Show(Detalhes(encontrado), "Contato encontrado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AlterarContato()
        {
            Contato atual = ContatoSelecionado();
            if (atual == null)
            {
                MessageBox.Show("Selecione um contato na lista (ou use Pesquisar).", "Alterar contato",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (FormContato f = new FormContato(atual))
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                if (contatos.alterar(f.Resultado))
                {
                    ListarContatos();
                    SelecionarNaLista(f.Resultado);
                }
                else
                {
                    MessageBox.Show("Não foi possível alterar o contato.", "Alterar contato",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RemoverContato()
        {
            Contato c = ContatoSelecionado();
            if (c == null)
            {
                MessageBox.Show("Selecione um contato na lista (ou use Pesquisar).", "Remover contato",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult r = MessageBox.Show($"Remover o contato \"{c.Nome}\"?", "Remover contato",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;

            if (contatos.remover(c))
                ListarContatos();
            else
                MessageBox.Show("Contato não encontrado.", "Remover contato",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ListarContatos()
        {
            lvContatos.Items.Clear();

            foreach (Contato c in contatos.Agenda)
            {
                ListViewItem item = new ListViewItem(c.Nome);
                item.SubItems.Add(c.Email);
                item.SubItems.Add(c.DtNasc.ToString());
                item.SubItems.Add(c.getIdade().ToString());
                item.SubItems.Add(c.getTelefonePrincipal());
                item.Tag = c;
                lvContatos.Items.Add(item);
            }

            lblTotal.Text = $"Total de contatos: {contatos.Agenda.Count}";
        }

        // ---------- Auxiliares ----------

        private Contato ContatoSelecionado()
        {
            if (lvContatos.SelectedItems.Count == 0) return null;
            return (Contato)lvContatos.SelectedItems[0].Tag;
        }

        private void SelecionarNaLista(Contato c)
        {
            foreach (ListViewItem item in lvContatos.Items)
            {
                if (c.Equals(item.Tag))
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                    break;
                }
            }
        }

        private static string Detalhes(Contato c)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Nome: {c.Nome}");
            sb.AppendLine($"E-mail: {c.Email}");
            sb.AppendLine($"Nascimento: {c.DtNasc} ({c.getIdade()} anos)");

            if (c.Telefones.Count == 0)
            {
                sb.Append("Telefones: nenhum");
            }
            else
            {
                sb.AppendLine("Telefones:");
                foreach (Telefone t in c.Telefones)
                    sb.AppendLine($"  - {t}");
            }

            return sb.ToString();
        }
    }
}
