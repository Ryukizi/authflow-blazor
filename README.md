# AuthFlow

O **AuthFlow** é uma aplicação completa de autenticação e controle de acesso, com foco em segurança, organização e boas práticas de desenvolvimento.

O sistema permite o gerenciamento de usuários com cadastro validado por e-mail, login seguro, controle de sessão e autorização baseada em perfis (usuário e administrador).

O projeto foi desenvolvido com foco em arquitetura limpa, separação de responsabilidades e escalabilidade.

---

## 🚀 Funcionalidades

* Cadastro de usuários com validação por código enviado por e-mail
* Autenticação de login
* Logout seguro
* Hash de senhas para armazenamento seguro
* Persistência de sessão do usuário
* Controle de acesso baseado em perfis (Role-Based Access Control)
* Área de acesso restrito para administradores
* Integração com banco de dados via Supabase

---

## 🧱 Tecnologias e ferramentas

* C#
* .NET
* Blazor
* HTML5
* CSS3
* Bootstrap
* Supabase

---

## 🏗️ Arquitetura e organização

O projeto segue uma estrutura modular com separação clara de responsabilidades:

* **Models** → Representação das entidades e dados do banco
* **Services** → Regras de negócio e lógica de autenticação
* **Pages** → Interface e componentes de UI

Essa estrutura foi pensada para facilitar manutenção, escalabilidade e legibilidade do código.

---

## 🔐 Segurança

* Senhas armazenadas utilizando hashing
* Verificação de e-mail no cadastro
* Controle de acesso por perfil de usuário (Admin/User)
* Gerenciamento de sessão para autenticação persistente

---

## 🧠 Conceitos aplicados

* Autenticação e autorização de usuários
* Gerenciamento de sessões em aplicações web
* Integração com banco de dados (Supabase)
* Arquitetura em camadas
* Separação de responsabilidades (Clean Architecture básica)
* Boas práticas de desenvolvimento com .NET

---

## 📌 Observação

Este projeto foi desenvolvido como parte do processo de aprendizado em desenvolvimento web com .NET, com foco em evolução prática e construção de portfólio técnico.

