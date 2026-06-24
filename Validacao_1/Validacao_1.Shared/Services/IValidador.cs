using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Validacao_1.Shared.Models;

namespace Validacao_1.Shared.Services
{
    public interface IValidador
    {
        public Task<List<string>> EntradaDeDados(Pessoa pessoa);

        public bool Idade(Pessoa pessoa);

        public string Nome(Pessoa pessoa);

        public Task<string> Email(Pessoa pessoa);

        public string Senha(Pessoa pessoa);
    }
}
