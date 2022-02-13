namespace CodeMatrix.Mepd.Application.Identity.Users;

public class UserCreateRequest
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsActive { get; set; }
}

public class UserCreateRequestValidator : CustomValidator<UserCreateRequest>
{
    private readonly IUserService _userService = default;
    private readonly IStringLocalizer<UserCreateRequestValidator> _localizer;
    private readonly ILogger<UserCreateRequestValidator> _logger;

    public UserCreateRequestValidator(
        IUserService userService,
        IStringLocalizer<UserCreateRequestValidator> localizer,
        ILogger<UserCreateRequestValidator> logger)
    {
        _userService = userService;
        _localizer = localizer;
        _logger = logger;

        RuleFor(e => e.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(3)
            .MustAsync(async (user, username, _) =>
            {
                var status = await _userService.ExistsWithNameAsync(username);
                return !status;
            })
            .WithMessage((v, value) => string.Format(_localizer["identity.alreadyExistsUsername"], value));

        RuleFor(e => e.FirstName)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(e => e.LastName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(e => e.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress().WithMessage("identity.invalidEmail")
            .MustAsync(async (user, email, _) => !await _userService.ExistsWithEmailAsync(email))
            .WithMessage((m, v) => string.Format(_localizer["identity.alreadyExistsEmail"], v));

    }
}