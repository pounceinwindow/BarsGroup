using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebApplication1.Components.Shared;

public class CustomValidation : ComponentBase
{
    private ValidationMessageStore? _messageStore;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException("CustomValidation must be used within an EditForm.");
        }

        _messageStore = new ValidationMessageStore(CurrentEditContext);

        CurrentEditContext.OnValidationRequested += (s, e) => _messageStore?.Clear();
        CurrentEditContext.OnFieldChanged += (s, e) => _messageStore?.Clear(e.FieldIdentifier);
    }

    public void DisplayErrors(IDictionary<string, IEnumerable<string>> errors)
    {
        if (CurrentEditContext == null || _messageStore == null) return;
        
        _messageStore.Clear();
        foreach (var err in errors)
        {
            var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, err.Key);
            _messageStore.Add(fieldIdentifier, err.Value);
        }

        CurrentEditContext.NotifyValidationStateChanged();
    }

    public void ClearErrors()
    {
        _messageStore?.Clear();
        CurrentEditContext?.NotifyValidationStateChanged();
    }
}
