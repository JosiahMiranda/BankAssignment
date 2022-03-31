using McbaExample.Data;
using McbaExample.Models;
using MvcMovie.Models.Repository;

namespace MvcMovie.Models.DataManager;

public class LoginManager : IDataRepository<Login, string>
{
    private readonly McbaContext _context;

    public LoginManager(McbaContext context)
    {
        _context = context;
    }

    public Login Get(string id)
    {
        return _context.Logins.Find(id);
    }

    public IEnumerable<Login> GetAll()
    {
        return _context.Logins.ToList();
    }

    public string Add(Login login)
    {
        _context.Logins.Add(login);
        _context.SaveChanges();

        return login.LoginID;
    }

    public string Delete(string id)
    {
        _context.Logins.Remove(_context.Logins.Find(id));
        _context.SaveChanges();

        return id;
    }

    public string Update(string id, Login login)
    {
        _context.Update(login);
        _context.SaveChanges();
            
        return id;
    }

    public string LockOrUnlock(string id)
    {
        var login = _context.Logins.Find(id);
        if (login.LoginStatus == LoginStatus.Unlocked)
        {
            login.LoginStatus = LoginStatus.Locked;
        } else
        {
            login.LoginStatus = LoginStatus.Unlocked;
        }
        _context.Update(login);
        _context.SaveChanges();

        return id;
    }
}
