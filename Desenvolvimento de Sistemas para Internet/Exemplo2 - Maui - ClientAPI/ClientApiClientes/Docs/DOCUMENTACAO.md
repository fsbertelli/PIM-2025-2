# Documentação do Projeto: ClientApiClientes

Este documento detalha a estrutura, os componentes e as funcionalidades do projeto MAUI `ClientApiClientes`. O objetivo é servir como um guia de aprendizado sobre o desenvolvimento de aplicativos com .NET MAUI.

## 1. Estrutura de Pastas

O projeto segue uma arquitetura baseada em separação de responsabilidades, refletida na seguinte estrutura de pastas:

-   **/Models**: Contém as classes de modelo de dados (POCOs). Essas classes representam as entidades do sistema.
-   **/Views**: Contém as páginas da interface do usuário (UI) e seus respectivos códigos de lógica (code-behind).
-   **/Services**: Contém as classes de serviço que encapsulam a lógica de negócio e o acesso a dados.
-   **/Docs**: Contém esta documentação.

## 2. Análise de Layouts das Páginas

### 2.1. ClientesPage.xaml

A tela de listagem de clientes foi projetada para ter uma aparência moderna, abandonando uma tabela tradicional em favor de um layout de "cards".

-   **`Grid` Principal**: Divide a tela para a lista e para o botão de adicionar flutuante.
-   **`ScrollView`**: Garante que a lista de clientes possa ser rolada verticalmente.
-   **`VerticalStackLayout` com `BindableLayout`**: Renderiza um `DataTemplate` para cada item na coleção de dados. É uma alternativa leve ao `CollectionView`.
-   **`DataTemplate`**: Define a aparência de um único item (o card de cliente).
-   **`Frame`**: Cria o efeito de "card" com borda, sombra e fundo.
-   **Botão Flutuante (FAB)**: O botão `+` estilizado e posicionado no canto inferior direito para a ação primária de adicionar um novo cliente.

### 2.2. CreateCliente.xaml

O layout desta página é um formulário vertical clássico, projetado para simplicidade e clareza.

-   **`ScrollView`**: O elemento raiz é um `ScrollView`, que permite que o formulário seja rolado em telas menores, evitando que os campos fiquem inacessíveis.
-   **`VerticalStackLayout`**: Dentro do `ScrollView`, um `VerticalStackLayout` empilha todos os elementos do formulário (rótulos e campos de entrada) um abaixo do outro, com um espaçamento (`Spacing`) definido para manter a consistência.
-   **`Label` e `Entry`/`Picker`**: Para cada campo de dados, usamos um par de `Label` (para o título do campo) e um controle de entrada (`Entry` para texto, `DatePicker` para data, `Picker` para seleção). Esta é a abordagem mais direta e robusta para criar formulários em MAUI.

### 2.3. SettingsPage.xaml

Similar à página de criação, esta tela usa um `ScrollView` e um `VerticalStackLayout` para apresentar um formulário. A principal diferença está no uso de um `Grid` para alinhar perfeitamente os rótulos e os campos de entrada em colunas.

-   **`Grid`**: É configurado com duas colunas (`Auto` para o `Label` e `*` para o `Entry`), garantindo que todos os campos de entrada comecem na mesma posição horizontal, criando uma aparência limpa e organizada, ideal para formulários de configuração.

## 3. Análise das Classes de Serviço

### 3.1. SettingsService.cs

Esta classe funciona como um "contêiner de estado" para as configurações da API.

-   **Propósito**: Manter a URL base e as rotas do CRUD em um único lugar, acessível por toda a aplicação.
-   **Tempo de Vida `Singleton`**: É registrada como um Singleton no `MauiProgram.cs`. Isso significa que existe apenas **uma instância** desta classe durante toda a execução do aplicativo. Qualquer alteração feita (pela `SettingsPage`) é refletida em todos os lugares que usam o serviço (como o `ClienteService`).
-   **Valores Padrão**: A classe contém valores padrão `const string` para todas as rotas. Isso garante que o aplicativo tenha uma configuração funcional logo de início.

### 3.2. ClienteService.cs

Esta é a classe que atua como uma ponte entre a interface do usuário e a API web.

-   **Propósito**: Abstrair os detalhes da comunicação HTTP. As páginas não sabem como fazer uma requisição web; elas apenas pedem ao `ClienteService` para "buscar os clientes" ou "salvar um cliente".
-   **`HttpClient`**: Usa uma instância de `HttpClient` para realizar as operações de rede (GET, POST, PUT, DELETE).
-   **Dependência do `SettingsService`**: Em vez de ter URLs fixas, ele recebe o `SettingsService` por injeção de dependência para construir a URL completa dinamicamente antes de cada chamada.
-   **Programação Assíncrona (`async`/`await`)**: Todos os métodos que envolvem comunicação de rede são assíncronos (`async Task`). Isso é crucial para que o aplicativo não "congele" enquanto espera a resposta da API.
-   **Manipulação de JSON**: Utiliza métodos de extensão como `ReadFromJsonAsync` e `PostAsJsonAsync` para converter (serializar/desserializar) os objetos C# para o formato JSON e vice-versa.

---

## 4. FAQ - Perguntas Frequentes

### Perguntas Gerais e de Navegação

#### P: Onde foi configurado que a tela inicial é a `ClientesPage`?

**R:** No arquivo `AppShell.xaml`. A primeira entrada `ShellContent` define a página inicial da aplicação.

#### P: O que a classe `Shell` faz e como ela é usada para navegar?

**R:** A `Shell` é o "contêiner" principal da aplicação, fornecendo a estrutura visual e um sistema de navegação unificado baseado em URI. Navegamos usando `Shell.Current.GoToAsync("caminho")` após registrar as rotas no `AppShell.xaml.cs`.

#### P: O que o atributo `[QueryProperty]` faz em `CreateCliente.xaml.cs`?

**R:** Ele mapeia um parâmetro passado durante a navegação para uma propriedade da página de destino. Usamos isso para passar o `ClienteId` para a tela de edição.

### Perguntas sobre Layout e Componentes

#### P: Como se usa o `BindableLayout`?

**R:** Ele permite que um layout (como `VerticalStackLayout`) gere seu conteúdo a partir de uma coleção de dados, usando as propriedades `ItemsSource` e `ItemTemplate`. É uma alternativa leve ao `CollectionView`.

#### P: O que é um `DataTemplate`? E um `Frame`?

**R:**
-   **`DataTemplate`**: É um "molde" que define a aparência de um objeto de dados.
-   **`Frame`**: É um controle de layout que desenha uma borda e uma sombra, usado para criar o efeito de "card".

### Perguntas sobre Serviços e Lógica

#### P: Como o Dependency Injection (Injeção de Dependência) funciona no projeto?

**R:** É configurada no `MauiProgram.cs`, onde "registramos" nossos serviços e páginas. O .NET MAUI se encarrega de criar e "injetar" as instâncias necessárias nos construtores das classes.

#### P: Qual a diferença entre `AddSingleton` e `AddTransient`?

**R:**
-   **`AddSingleton`**: Cria **uma única instância** do objeto para toda a aplicação (usado para os serviços).
-   **`AddTransient`**: Cria **uma nova instância** do objeto toda vez que ele é solicitado (usado para as páginas).

#### P: Como os botões "Editar" e "Excluir" dentro da lista sabem qual cliente selecionar?

**R:** Usando a propriedade `CommandParameter="{Binding .}"` no XAML, que atrela o objeto `Cliente` inteiro ao botão. No code-behind, o método do evento acessa essa informação a partir do `sender`.

#### P: Por que o `ClienteService` precisa de um `HttpClient`?

**R:** O `HttpClient` é a classe do .NET para enviar requisições HTTP e receber respostas de uma API web.

#### P: Como o `ClienteService` sabe qual URL chamar?

**R:** Ele solicita as informações ao `SettingsService`. Ele pega a `BaseUrl` e a rota específica (ex: `DeleteEndpoint`) e os combina para formar a URL final, tornando o serviço reutilizável.

#### P: Por que o `SettingsService` é um `Singleton`?

**R:** Para garantir que haja uma única fonte de verdade para as configurações da API. Se um usuário salva uma nova URL, a instância única é atualizada, e o `ClienteService` usará essa nova configuração imediatamente.

#### P: Como eu poderia fazer as configurações serem salvas permanentemente?

**R:** Atualmente, as configurações são perdidas quando o aplicativo é fechado. Para torná-las permanentes, você poderia usar a API **`Preferences`** do .NET MAUI. No `SettingsService`, ao salvar, você usaria `Preferences.Set("api_base_url", BaseUrl)`. No construtor do serviço, você usaria `BaseUrl = Preferences.Get("api_base_url", DefaultBaseUrl)` para carregar o valor salvo.