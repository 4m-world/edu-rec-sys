namespace CodeMatrix.Mepd.Application.Identity.Users;

public class UserUpdateRequest
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool? IsEmailConfirmed { get; set; }
    public bool? IsPhoneNumberConfirmed { get; set; }
    public bool? IsActive { get; set; }
}

public class UserUpdateRequestValidator : CustomValidator<UserUpdateRequest>
{
    private readonly IUserService _userService = default;
    private readonly IStringLocalizer<UserUpdateRequestValidator> _localizer;
    private readonly ILogger<UserUpdateRequestValidator> _logger;

    public UserUpdateRequestValidator(
        IUserService userService,
        IStringLocalizer<UserUpdateRequestValidator> localizer,
        ILogger<UserUpdateRequestValidator> logger)
    {
        _userService = userService;
        _localizer = localizer;
        _logger = logger;


        RuleFor(e => e.FirstName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(e => e.LastName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(e => e.Email)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .EmailAddress().WithMessage("identity.invalidEmail")
           .MustAsync(async (user, email, _) => !await _userService.ExistsWithEmailAsync(email, user.Id))
            .WithMessage((m, v) => string.Format(_localizer["identity.alreadyExistsEmail"], v));

    }
}