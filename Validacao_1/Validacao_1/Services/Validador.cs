using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Supabase;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Validacao_1.Shared.Services;
using static Validacao_1.Shared.Pages.Home;

namespace Validacao_1.Services
{
    public class Validador : IValidador
    {
        // 1. Mantemos o nome correto do cliente injetado
        private readonly Client _supabaseClient;

        public Validador(Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<List<string>> EntradaDeDados(Pessoa pessoa)
        {
            var mensagem = new List<string>();

            if (Idade(pessoa))
            {
                // Como Email agora retorna a resposta, adicionamos o retorno dele na lista
                mensagem.Add(await Email(pessoa));
                mensagem.Add(Senha(pessoa));
                mensagem.Add(Nome(pessoa));
            }
            else
            {
                mensagem.Add("Pessoa é menor de idade, não é possível validar email e senha.");
            }
            return mensagem;
        }

        public bool Idade(Pessoa pessoa)
        {
            bool maiorIdade = false;
            if (pessoa.Idade >= 18)
            {
                maiorIdade = true;
            }
            return maiorIdade;
        }

        public string Nome(Pessoa pessoa)
        {
            bool ValidName = !string.IsNullOrWhiteSpace(pessoa.Nome) && pessoa.Nome.All(c => char.IsLetter(c) || c == ' ');

            bool temEspacoDuplicado = Regex.IsMatch(pessoa.Nome ?? string.Empty, @"\s{2,}");
            if (ValidName && !temEspacoDuplicado)
            {
                return "Nome valído";
            }
            else
            {
                return "Nome invalído";
            }
        }

        public async Task<string> Email(Pessoa pessoa)
        {
            bool emailValido = Regex.IsMatch(pessoa.Email ?? string.Empty, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!emailValido)
            {
                return "Formatado de email inválido";
            }
            try
            {
                var resultado = await _supabaseClient
                    .From<Usuario>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, pessoa.Email)
                    .Get();

                if (resultado.Models.Any())
                {
                    return "Email já cadastrado no banco de dados";
                }

                return "Email válido";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERRO COMPLETO DO BANCO: {ex.ToString()} ===");
                return "Erro ao validar email no banco de dados";
            }
        }
        public async Task EnviarEmailCodigo(string emailDoCliente, int codigo)
        {


            var mensagem = new MailMessage();
            mensagem.From = new MailAddress(remetente, "Sistema de Cadastro");
            mensagem.To.Add(new MailAddress(emailDoCliente));
            mensagem.Subject = "Código de Verificação";
            mensagem.Body = $"Seu código de verificação é: {codigo}";
            mensagem.IsBodyHtml = false;

            using var smtpClient = new SmtpClient("smtp-relay.brevo.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("", suaSenhaDeApp),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(mensagem);
        }
        }

        public string Senha(Pessoa pessoa)
        {
            bool senhaValida = Regex.IsMatch(pessoa.Senha ?? string.Empty, @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");

            if (senhaValida)
            {
                return "senha valída";
            }
            return "senha inválida";
        }

        public string HashPassword(string password)
        {
            const int iterations = 100_000;
            const int saltSize = 32;
            const int keySize = 32;

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[saltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(keySize);

            string saltBase64 = Convert.ToBase64String(salt);
            string KeyBase64 = Convert.ToBase64String(key);

            return $"{iterations}.{saltBase64}.{KeyBase64}";
        }

        public bool VerifyPassword(string password, string storehash)
        {
            var parts = storehash.Split('.');
            if (parts.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(parts[0], out int iterations))
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[1]);
            var storedKey = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var enteredKey = pbkdf2.GetBytes(storedKey.Length);

            return CryptographicOperations.FixedTimeEquals(enteredKey, storedKey);
        }

        public async Task FazerLogout()
        {
            await _supabaseClient.Auth.SignOut();
        }
    }
}
