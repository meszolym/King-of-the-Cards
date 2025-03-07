namespace KC.Backend.Models.Utils.Interfaces
{
    public interface IIdentityBearer<T>
    {
        T Id { get; }
    }
}
