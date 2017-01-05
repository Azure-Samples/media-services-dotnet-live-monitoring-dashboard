namespace MediaDashboard.Api.Models
{
    public interface IInformationRepository
    {
        object Get();

        T GetAs<T>() where T : class;
    }

    public abstract class AbstractInformationRepository : IInformationRepository
    {
        public abstract object Get();

        public virtual T GetAs<T>() where T : class
        {
            return Get() as T;
        }
    }
}