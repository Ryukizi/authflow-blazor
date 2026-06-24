using Supabase;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration; // Adicionado para ler o appsettings
using Validacao_1.Shared.Services;
using Validacao_1.Shared.Models;

namespace Validacao_1.Web.Services
{
    public class Validador : IValidador
    {
        private readonly Client _supabaseClient;
        private readonly IConfiguration _config; // Injeção de configuração

        public Validador(Client supabaseClient, IConfiguration config)
        {
            _supabaseClient = supabaseClient;
            _config = config;
        }

        public async Task<List<string>> EntradaDeDados(Pessoa pessoa)
        {
            var erros = new List<string>();

            if (!Idade(pessoa))
            {
                erros.Add("Pessoa é menor de idade, não é possível realizar o cadastro.");
                return erros; // Retorna imediatamente pois menor de idade não pode se cadastrar
            }

            //  Validações adicionando apenas se houver erro
            var erroNome = Nome(pessoa);
            if (!string.IsNullOrEmpty(erroNome)) erros.Add(erroNome);

            var erroEmail = await Email(pessoa);
            if (!string.IsNullOrEmpty(erroEmail)) erros.Add(erroEmail);

            var erroSenha = Senha(pessoa);
            if (!string.IsNullOrEmpty(erroSenha)) erros.Add(erroSenha);

            return erros;
        }

        public bool Idade(Pessoa pessoa)
        {
            return pessoa.Idade >= 18;
        }

        public string Nome(Pessoa pessoa)
        {
            bool validName = !string.IsNullOrWhiteSpace(pessoa.Nome) && pessoa.Nome.All(c => char.IsLetter(c) || c == ' ');
            bool temEspacoDuplicado = Regex.IsMatch(pessoa.Nome ?? string.Empty, @"\s{2,}");

            if (validName && !temEspacoDuplicado)
            {
                return null;
            }
            return "Nome inválido";
        }

        public async Task<string> Email(Pessoa pessoa)
        {
            bool emailValido = Regex.IsMatch(pessoa.Email ?? string.Empty, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!emailValido)
            {
                return "Formato de e-mail inválido";
            }
            try
            {
                var resultado = await _supabaseClient
                    .From<Usuario>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, pessoa.Email)
                    .Get();

                if (resultado.Models.Any())
                {
                    return "E-mail já cadastrado no banco de dados";
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERRO COMPLETO DO BANCO: {ex} ===");
                return "Erro ao validar e-mail no banco de dados";
            }
        }

        public string Senha(Pessoa pessoa)
        {
            bool senhaValida = Regex.IsMatch(pessoa.Senha ?? string.Empty, @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");

            if (senhaValida)
            {
                return null;
            }
            return "Senha inválida";
        }
    }
}