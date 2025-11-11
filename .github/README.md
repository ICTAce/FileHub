# GitHub Copilot Chat Modes

This directory contains instructions for GitHub Copilot to provide context-aware assistance based on your development task.

## Available Chat Modes

The following chat modes are automatically activated based on the files you're working with:

### 🎨 Blazor Developer
- **Purpose**: Building Blazor components with Syncfusion in Oqtane
- **Activated when**: Working with `.razor` files or Blazor components
- **Expertise**: Component lifecycle, data binding, state management, Syncfusion components, Oqtane patterns

### 🎨 HTML Designer
- **Purpose**: Creating and styling with HTML and CSS
- **Activated when**: Working with `.html`, `.css`, or `.scss` files
- **Expertise**: Semantic HTML, modern CSS, responsive design, accessibility, theming

### 💻 .NET Backend Developer
- **Purpose**: Implementing databases and REST APIs in Oqtane
- **Activated when**: Working with controllers, services, repositories, or Entity Framework
- **Expertise**: Repository pattern, REST APIs, Entity Framework Core, Oqtane security

### 🚀 DevOps Engineer
- **Purpose**: Setting up and managing CI/CD pipelines
- **Activated when**: Working with `.yml`, `.yaml` files, Docker, or deployment scripts
- **Expertise**: GitHub Actions, Docker, CI/CD, infrastructure as code, deployment strategies

### 🧪 Tester
- **Purpose**: Writing and executing tests
- **Activated when**: Working with test files or testing frameworks
- **Expertise**: BUnit (Blazor components), TUnit (unit tests), Reqnroll (BDD), test patterns

## How It Works

GitHub Copilot automatically reads the `copilot-instructions.md` file and uses it to provide contextual assistance. The instructions are designed to:

1. **Detect context**: Recognize which type of task you're working on based on file types and patterns
2. **Provide specialized help**: Offer guidance specific to that domain (Blazor, backend, DevOps, etc.)
3. **Follow best practices**: Suggest code that follows Oqtane and .NET conventions
4. **Include examples**: Show practical examples relevant to the FileHub project

## Using Chat Modes

You don't need to do anything special - just start coding! Copilot will:
- Suggest code completions based on the current context
- Answer questions with domain-specific knowledge when you use Copilot Chat
- Provide examples that match the project's patterns and conventions

## Tips

- **Be specific**: The more context you provide in your code comments and file names, the better Copilot can assist
- **Use comments**: Add comments describing what you want to achieve, and Copilot will suggest implementations
- **Ask questions**: In Copilot Chat, ask questions about specific patterns or approaches for your current task
- **Review suggestions**: Always review and test Copilot's suggestions to ensure they meet your needs

## Customization

To modify or extend these chat modes:
1. Edit `.github/copilot-instructions.md`
2. Add new sections for additional modes or update existing guidelines
3. Commit and push your changes - GitHub Copilot will automatically use the updated instructions

## Learn More

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [Oqtane Documentation](https://docs.oqtane.org/)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
