# GitHub Copilot Chat Modes

This directory contains individual instruction files for GitHub Copilot to provide specialized, context-aware assistance based on your development task.

## Available Chat Modes

The following chat modes are available as separate files in the `copilot-modes/` directory:

### 🎨 Blazor Developer (`blazor-developer.md`)
- **Purpose**: Building Blazor components with Syncfusion in Oqtane
- **Activated when**: Working with `.razor` files or Blazor components
- **Expertise**: Component lifecycle, data binding, state management, Syncfusion components, Oqtane patterns

### 🎨 HTML Designer (`html-designer.md`)
- **Purpose**: Creating and styling with HTML and CSS
- **Activated when**: Working with `.html`, `.css`, or `.scss` files
- **Expertise**: Semantic HTML, modern CSS, responsive design, accessibility, theming

### 💻 .NET Backend Developer (`backend-developer.md`)
- **Purpose**: Implementing databases and REST APIs in Oqtane
- **Activated when**: Working with controllers, services, repositories, or Entity Framework
- **Expertise**: Repository pattern, REST APIs, Entity Framework Core, Oqtane security

### 🚀 DevOps Engineer (`devops-engineer.md`)
- **Purpose**: Setting up and managing CI/CD pipelines
- **Activated when**: Working with `.yml`, `.yaml` files, Docker, or deployment scripts
- **Expertise**: GitHub Actions, Docker, CI/CD, infrastructure as code, deployment strategies

### 🧪 Tester (`tester.md`)
- **Purpose**: Writing and executing tests
- **Activated when**: Working with test files or testing frameworks
- **Expertise**: BUnit (Blazor components), TUnit (unit tests), Reqnroll (BDD), test patterns

## How to Use in Visual Studio Code

Each chat mode is in a separate file that you can open and use with GitHub Copilot:

1. **Open the relevant mode file**: Navigate to `.github/copilot-modes/` and open the file for your current task:
   - `blazor-developer.md` for Blazor components
   - `html-designer.md` for HTML/CSS work
   - `backend-developer.md` for API and database work
   - `devops-engineer.md` for CI/CD and infrastructure
   - `tester.md` for writing tests

2. **Reference in Copilot Chat**: With the file open, GitHub Copilot can use it as context. You can:
   - Use `@workspace` in Copilot Chat to reference workspace files
   - Add the mode file to your chat context by mentioning it
   - Keep the mode file open in a tab while working on related code

3. **Switch modes easily**: Simply open a different mode file when switching tasks

## Using Chat Modes

The chat mode files provide:
- **Context-aware guidance**: Specialized instructions for each development domain
- **Code examples**: Practical patterns specific to FileHub and Oqtane
- **Best practices**: Guidelines following .NET, Blazor, and Oqtane conventions
- **Project context**: Information about FileHub's technology stack

## Tips

- **Open the relevant mode file** before asking Copilot Chat questions about that domain
- **Keep mode files open** in separate tabs to quickly switch between contexts
- **Reference examples** from the mode files when asking for code suggestions
- **Combine modes** by opening multiple files when working across domains
- **Use comments** to describe what you want to achieve, following patterns from the mode files

## Customization

To modify or extend these chat modes:
1. Edit the individual files in `.github/copilot-modes/`
2. Add new mode files for additional development contexts
3. Update this README to document new modes
4. Commit and push your changes

## Learn More

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [Oqtane Documentation](https://docs.oqtane.org/)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
