using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Guards;
using SneakersShop.Domain.Common.ValueObjects;
using SneakersShop.Domain.Consumer.DomainEvents;

namespace SneakersShop.Domain.Consumer;


/*
 * In a feature add: 
 * Wishlists, Reviews, etc.
 */
public sealed class UserProfile : AggregateRoot
{
    public string Name { get; private set; }
    public string LastName { get; private set; }
    public Address? DefaultAddress { get; private set; }
    public bool IsFlagged { get; private set; }
    private UserProfile() { }

    private UserProfile(
        Guid id,
        Address? defaultAddress,
        bool isFlagged,
        string name,
        string lastName)
        : base(id)
    {
        DefaultAddress = defaultAddress;
        IsFlagged = isFlagged;
        Name = name;
        LastName = lastName;
    }

    public static UserProfile Create(
        Guid id,
        Address? defaultAddress,
        string name,
        string lastName)
    {
        Guard.Against.Empty(id);
        Guard.Against.NullOrEmpty(name);
        Guard.Against.NullOrEmpty(lastName);
        bool isFlagged = false;

        return new UserProfile(id, defaultAddress, isFlagged, name, lastName);
    }

    public void UpdateDefaultAddress(Address? newAddress)
    {
        DefaultAddress = newAddress;
        Touch();
    }

    public void FlagUser()
    {
        if (IsFlagged)
            return;
        IsFlagged = true;
        // Implement domain event for user flagged
        // when a user is flagged, you might want to notify other parts of the system.
        // Now we don't have a listner for this event, but we can still add the domain event to the list of events.
        //AddDomainEvent(new UserFlaggedDomainEvent(Id));
        Touch();
    }

    public void UnflagUser()
    {
        if (!IsFlagged)
            return;
        IsFlagged = false;
        Touch();
    }

    public void UpdateName(string newName)
    {
        Guard.Against.NullOrEmpty(newName);
        Name = newName;
        Touch();
    }

    public void UpdateLastName(string newLastName)
    {
        Guard.Against.NullOrEmpty(newLastName);
        LastName = newLastName;
        Touch();
    }
}