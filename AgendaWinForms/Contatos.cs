using System.Collections.Generic;

namespace Agenda
{
    public class Contatos
    {
        private readonly List<Contato> agenda = new List<Contato>();

        public IReadOnlyList<Contato> Agenda => agenda;

        // Retorna false se for nulo ou se já existir contato igual (mesmo e-mail).
        public bool adicionar(Contato c)
        {
            if (c == null || agenda.Contains(c)) return false;

            agenda.Add(c);
            return true;
        }

        // Retorna o contato da agenda igual a c, ou null se não existir.
        public Contato pesquisar(Contato c)
        {
            if (c == null) return null;

            int i = agenda.IndexOf(c);
            return i >= 0 ? agenda[i] : null;
        }

        // Substitui o contato existente (mesmo e-mail) pelos novos dados.
        public bool alterar(Contato c)
        {
            if (c == null) return false;

            int i = agenda.IndexOf(c);
            if (i < 0) return false;

            agenda[i] = c;
            return true;
        }

        public bool remover(Contato c)
        {
            if (c == null) return false;

            return agenda.Remove(c);
        }
    }
}
