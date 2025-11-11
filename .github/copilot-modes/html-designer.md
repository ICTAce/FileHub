# 🎨 HTML Designer Mode

**Activated when**: Working with `.html`, `.css`, `.scss` files, or styling tasks

## Project Context

FileHub is an Oqtane module built with:
- **Frontend**: Blazor WebAssembly (.NET 9.0) with Syncfusion components
- **Backend**: ASP.NET Core (.NET 9.0) with Entity Framework Core
- **Framework**: Oqtane CMS 6.2.1
- **Testing**: BUnit, TUnit, and Reqnroll

## Guidelines

- Write semantic HTML5 markup
- Use modern CSS features (Flexbox, Grid, Custom Properties)
- Follow BEM methodology for CSS class naming when appropriate
- Ensure responsive design using mobile-first approach
- Use CSS variables for theming and consistency
- Implement accessibility features (ARIA labels, semantic elements, keyboard navigation)
- Optimize for performance (minimize reflows, use CSS animations over JavaScript)
- Follow Oqtane's theming conventions
- Use utility-first CSS classes when available
- Ensure cross-browser compatibility

## Example Patterns

```css
:root {
    --primary-color: #0066cc;
    --spacing-unit: 8px;
}

.file-hub__container {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: calc(var(--spacing-unit) * 2);
    padding: var(--spacing-unit);
}

.file-hub__item {
    border: 1px solid #e0e0e0;
    border-radius: 4px;
    transition: transform 0.2s ease;
}

.file-hub__item:hover {
    transform: translateY(-2px);
}
```

## General Development Guidelines

**For all modes**:
- Write clean, readable, and maintainable code
- Follow SOLID principles
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Handle exceptions appropriately
- Use async/await for I/O operations
- Follow the existing code style and conventions in the project
- Consider security implications (XSS, SQL injection, CSRF)
- Implement proper logging for debugging and monitoring
- Write self-documenting code with comments only where necessary

**Oqtane-Specific Conventions**:
- Follow Oqtane's module structure and conventions
- Use Oqtane's built-in services (ISettingService, ILogService, IUserPermissions)
- Implement proper permission checks using `[Authorize]` attributes
- Follow Oqtane's localization patterns for multi-language support
- Use Oqtane's notification system for user feedback
