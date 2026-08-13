using FluentAssertions;

using SneakersShop.Domain.Common.ValueObjects;
using SneakersShop.Domain.Consumer;

namespace SneakersShop.Domain.UnitTests.Users;

public class UserProfileTests
{
    private readonly Guid _validId = Guid.NewGuid();
    private readonly string _validName = "John";
    private readonly string _validLastName = "Doe";

    [Fact]
    public void Create_WithValidData_CreatesProfileAndSetsIsFlaggedToFalse()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);

        profile.Should().NotBeNull();
        profile.Id.Should().Be(_validId);
        profile.Name.Should().Be(_validName);
        profile.LastName.Should().Be(_validLastName);
        profile.IsFlagged.Should().BeFalse("a new user profile should not be flagged by default");
    }

    [Fact]
    public void Create_WithEmptyId_ThrowsException()
    {
        Action act = () => UserProfile.Create(Guid.Empty, null, _validName, _validLastName);

        act.Should().Throw<ArgumentException>("the provided Id is empty");
    }

    [Fact]
    public void Create_WithNullName_ThrowsException()
    {
        Action act = () => UserProfile.Create(_validId, null, null!, _validLastName);

        act.Should().Throw<ArgumentException>("Name cannot be null");
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsException()
    {
        Action act = () => UserProfile.Create(_validId, null, string.Empty, _validLastName);

        act.Should().Throw<ArgumentException>("Name cannot be empty");
    }

    [Fact]
    public void Create_WithNullLastName_ThrowsException()
    {
        Action act = () => UserProfile.Create(_validId, null, _validName, null!);

        act.Should().Throw<ArgumentException>("LastName cannot be null");
    }

    [Fact]
    public void Create_WithEmptyLastName_ThrowsException()
    {
        Action act = () => UserProfile.Create(_validId, null, _validName, string.Empty);

        act.Should().Throw<ArgumentException>("LastName cannot be empty");
    }


    [Fact]
    public void FlagUser_WhenNotFlagged_SetsIsFlaggedToTrue()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);

        profile.FlagUser();

        profile.IsFlagged.Should().BeTrue();
    }

    [Fact]
    public void FlagUser_Twice_IsIdempotent()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);
        profile.FlagUser();

        profile.FlagUser();

        profile.IsFlagged.Should().BeTrue("calling FlagUser multiple times should safely do nothing on subsequent calls");
    }

    [Fact]
    public void UnflagUser_WhenFlagged_SetsIsFlaggedToFalse()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);
        profile.FlagUser();

        profile.UnflagUser();

        profile.IsFlagged.Should().BeFalse();
    }

    [Fact]
    public void UnflagUser_WhenAlreadyNotFlagged_IsIdempotent()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);

        profile.UnflagUser();

        profile.IsFlagged.Should().BeFalse("calling UnflagUser on an unflagged profile should safely do nothing");
    }

    [Fact]
    public void UpdateDefaultAddress_WithNewAddress_UpdatesSuccessfully()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);

        var newAddress = new Address(
            country: "Germany",
            state: "Bavaria",
            city: "Erlangen",
            street: "Hauptstrasse",
            houseNumber: "12A",
            zipCode: "91054"
        );

        profile.UpdateDefaultAddress(newAddress);

        profile.DefaultAddress.Should().NotBeNull();
        profile.DefaultAddress.Should().Be(newAddress, "the profile's default address should be updated to the provided address");

        profile.DefaultAddress!.City.Should().Be("Erlangen");
    }

    [Fact]
    public void UpdateName_WithValidName_UpdatesSuccessfully()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);
        var newName = "Michael";

        profile.UpdateName(newName);

        profile.Name.Should().Be(newName);
    }

    [Fact]
    public void UpdateName_WithNullName_ThrowsException()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);

        Action act = () => profile.UpdateName(null!);

        act.Should().Throw<ArgumentException>("the new name cannot be null");
    }

    [Fact]
    public void UpdateName_WithEmptyName_ThrowsException()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);

        Action act = () => profile.UpdateName(string.Empty);

        act.Should().Throw<ArgumentException>("the new name cannot be empty");
    }

    [Fact]
    public void UpdateLastName_WithValidLastName_UpdatesSuccessfully()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);
        var newLastName = "Jordan";

        profile.UpdateLastName(newLastName);

        profile.LastName.Should().Be(newLastName);
    }

    [Fact]
    public void UpdateLastName_WithNullLastName_ThrowsException()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);

        Action act = () => profile.UpdateLastName(null!);

        act.Should().Throw<ArgumentException>("the new last name cannot be null");
    }

    [Fact]
    public void UpdateLastName_WithEmptyLastName_ThrowsException()
    {
        var profile = UserProfile.Create(_validId, null, _validName, _validLastName);

        Action act = () => profile.UpdateLastName(string.Empty);

        act.Should().Throw<ArgumentException>("the new last name cannot be empty");
    }
}