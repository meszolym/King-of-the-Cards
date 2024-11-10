namespace KC.Models.Interfaces
{
    public interface IIdentityBearer<T>
    {
        T Id { get; }
    }
}
