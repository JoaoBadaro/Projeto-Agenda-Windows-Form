using System;
using System.Collections.Generic;

namespace Agenda
{
    public class Contato
    {
        private string email;
        private string nome;
        private Data dtNasc;
        private List<Telefone> telefones;

        public Contato(string email, string nome, Data dtNasc)
        {
            this.email = email;
            this.nome = nome;
            this.dtNasc = dtNasc;
            this.telefones = new List<Telefone>();
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public Data DtNasc
        {
            get { return dtNasc; }
            set { dtNasc = value; }
        }

        public List<Telefone> Telefones
        {
            get { return telefones; }
        }

        public int getIdade()
        {
            if (dtNasc == null) return 0;

            DateTime hoje = DateTime.Today;
            int idade = hoje.Year - dtNasc.Ano;

            // Ainda não fez aniversário neste ano
            if (hoje.Month < dtNasc.Mes ||
                (hoje.Month == dtNasc.Mes && hoje.Day < dtNasc.Dia))
            {
                idade--;
            }

            return idade;
        }

        public void adicionarTelefone(Telefone t)
        {
            if (t == null) return;

            // Só pode existir um telefone principal
            if (t.Principal)
            {
                foreach (Telefone outro in telefones)
                    outro.Principal = false;
            }

            telefones.Add(t);
        }

        public string getTelefonePrincipal()
        {
            foreach (Telefone t in telefones)
            {
                if (t.Principal) return t.Numero;
            }

            // Sem principal definido: usa o primeiro, se houver
            return telefones.Count > 0 ? telefones[0].Numero : "";
        }

        public override string ToString()
        {
            string tel = getTelefonePrincipal();
            return $"Nome: {nome} | E-mail: {email} | Nascimento: {dtNasc} " +
                   $"| Idade: {getIdade()} | Telefone principal: {(tel == "" ? "não informado" : tel)}";
        }

        // Dois contatos são iguais quando possuem o mesmo e-mail.
        public override bool Equals(object obj)
        {
            if (obj is not Contato outro) return false;

            return string.Equals(email, outro.email,
                StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return (email ?? "").ToLowerInvariant().GetHashCode();
        }
    }
}
