# GitHub Copilot Custom Chat Modes

This directory contains custom chat modes for GitHub Copilot in Visual Studio Code. These modes provide specialized AI assistance tailored to specific development tasks in the FileHub project.

## What are Custom Chat Modes?

Custom chat modes (also called agents) are VS Code configurations that customize GitHub Copilot's behavior for specific tasks. When you activate a chat mode in VS Code, Copilot follows the instructions and guidelines defined in that mode's file.

## Available Chat Modes

The following custom chat modes are available in the `chatmodes/` directory:

### 🎨 Blazor Developer (`blazor-developer.chatmode.md`)
- **Description**: Expert in building Blazor components with Syncfusion in Oqtane CMS
- **Best for**: Creating `.razor` components, working with Syncfusion UI components, Oqtane module development
- **Tools**: File editing, search, codebase access

### 🎨 HTML Designer (`html-designer.chatmode.md`)
- **Description**: Expert in HTML5, CSS3, and modern responsive design for Oqtane themes
- **Best for**: Styling, creating responsive layouts, implementing accessibility features
- **Tools**: File editing, search, codebase access

### 💻 .NET Backend Developer (`backend-developer.chatmode.md`)
- **Description**: Expert in .NET backend development with Entity Framework Core and REST APIs
- **Best for**: Building controllers, services, repositories, database migrations, REST APIs
- **Tools**: File editing, search, codebase access

### 🚀 DevOps Engineer (`devops-engineer.chatmode.md`)
- **Description**: Expert in CI/CD pipelines, GitHub Actions, Docker, and deployment automation
- **Best for**: Creating workflows, Dockerfiles, deployment scripts, infrastructure as code
- **Tools**: File editing, search, codebase access

### 🧪 Tester (`tester.chatmode.md`)
- **Description**: Expert in testing with BUnit, TUnit, and Reqnroll
- **Best for**: Writing unit tests, component tests, BDD scenarios, test automation
- **Tools**: File editing, search, codebase access

## How to Use in Visual Studio Code

### Activating a Chat Mode

1. **Open GitHub Copilot Chat** in VS Code (View > Copilot Chat or `Ctrl+Alt+I`)
2. **Click the mode selector** at the top of the Copilot Chat panel
3. **Select your desired mode** from the dropdown list:
   - Blazor Developer
   - HTML Designer
   - .NET Backend Developer
   - DevOps Engineer
   - Tester
4. **Start chatting** - Copilot now follows the guidelines and expertise of that mode

### Switching Between Modes

- Simply select a different mode from the dropdown when you switch tasks
- Each mode persists until you change it or restart VS Code
- You can quickly switch between modes as you work on different parts of the codebase

### Using Chat Modes Effectively

1. **Select the right mode for your task**:
   - Working on a Blazor component? Use **Blazor Developer**
   - Styling a page? Use **HTML Designer**
   - Building an API endpoint? Use **.NET Backend Developer**
   - Setting up CI/CD? Use **DevOps Engineer**
   - Writing tests? Use **Tester**

2. **Ask specific questions**:
   - "Create a file upload component using SfUpload"
   - "How should I structure this REST API controller?"
   - "Write BUnit tests for this component"

3. **Request code generation**:
   - "Generate a repository class for File entity"
   - "Create a GitHub Actions workflow for building and testing"
   - "Write Reqnroll scenarios for file management"

4. **Get code reviews**:
   - "Review this Blazor component for best practices"
   - "Check this API controller for security issues"
   - "Improve the test coverage for this service"

## Benefits

✅ **Specialized expertise** for each development task  
✅ **Context-aware suggestions** based on FileHub's tech stack  
✅ **Consistent patterns** following Oqtane and .NET conventions  
✅ **Comprehensive examples** for common scenarios  
✅ **Production-ready code** with proper error handling, security, and best practices  
✅ **Easy mode switching** as you move between different tasks  

## Requirements

- Visual Studio Code with GitHub Copilot extension installed
- Active GitHub Copilot subscription
- VS Code version 1.85 or later (for custom chat modes support)

## Customization

To add or modify chat modes:

1. **Edit existing modes**: Modify the `.chatmode.md` files in `.github/chatmodes/`
2. **Add new modes**: Create a new `.chatmode.md` file with YAML frontmatter:
   ```markdown
   ---
   description: "Your mode description"
   tools: [editFiles, search, codebase]
   model: gpt-4
   ---
   
   # Your Mode Name
   
   Your instructions here...
   ```
3. **Restart VS Code**: The new modes will appear in the Copilot Chat mode selector
4. **Share with team**: Commit changes so everyone benefits from the customizations

## Learn More

- [VS Code Custom Chat Modes Documentation](https://code.visualstudio.com/docs/copilot/customization/custom-chat-modes)
- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [Awesome Copilot Chat Modes](https://github.com/dfinke/awesome-copilot-chatmodes)

## Learn More

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [Oqtane Documentation](https://docs.oqtane.org/)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
