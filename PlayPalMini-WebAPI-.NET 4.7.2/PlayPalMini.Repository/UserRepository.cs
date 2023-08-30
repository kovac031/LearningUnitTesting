using PlayPalMini.Common;
using PlayPalMini.Model;
using PlayPalMini.Repository.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PlayPalMini.Repository
{
    public class UserRepository : IUserRepository
    {
        public static string connectionString = "Data Source=VREMENSKISTROJ;Initial Catalog=PlayPalMini;Integrated Security=True";

        //-------------- GET ALL USERS ---------------------------
        public async Task<(List<RegisteredUser>, string)> GetAllAsync()
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString); 
                using (theConnection)
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM RegisteredUser", theConnection);
                    theConnection.Open();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<RegisteredUser> list = new List<RegisteredUser>();

                    if(reader.HasRows)
                    {
                        while (reader.Read()) 
                        {
                            RegisteredUser user = new RegisteredUser();
                            user.Id = reader.GetGuid(0);
                            user.Username = reader.GetString(1);
                            user.Pass = reader.GetString(2);
                            user.UserRole = reader.GetString(3);
                            user.UpdatedBy = reader.GetString(5);
                            user.DateUpdated = reader.GetDateTime(7);
                            user.CreatedBy = reader.GetString(4);
                            user.DateCreated = reader.GetDateTime(6); //brojevi su redoslijed stupca u tablici

                            list.Add(user);
                        }
                        reader.Close();
                        return (list, "Success");
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
        //-------------- GET ONE USER BY ID ---------------------------
        public async Task<(RegisteredUser, string)> GetOneByIdAsync(Guid id)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM RegisteredUser WHERE Id = @id", theConnection);
                    cmd.Parameters.AddWithValue("@id", id); //dodao id
                    theConnection.Open();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        RegisteredUser user = new RegisteredUser(); // mora biti tu da ga onaj return dole vidi

                        while (reader.Read())
                        {
                            //RegisteredUser user = new RegisteredUser();
                            user.Id = reader.GetGuid(0);
                            user.Username = reader.GetString(1);
                            user.Pass = reader.GetString(2);
                            user.UserRole = reader.GetString(3);
                            user.UpdatedBy = reader.GetString(5);
                            user.DateUpdated = reader.GetDateTime(7);
                            user.CreatedBy = reader.GetString(4);
                            user.DateCreated = reader.GetDateTime(6); //brojevi su redoslijed stupca u tablici
                        }
                        reader.Close();
                        return (user, "Success");
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
        //------------------ CREATE USER ---------------------
        public async Task<(bool, string)> CreateUserAsync(RegisteredUser user)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO RegisteredUser VALUES (@id, @username, @password, @role, @createdby, @updatedby, @timecreated, @timeupdated);", theConnection);

                    cmd.Parameters.AddWithValue("@id", user.Id = Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@password", user.Pass);
                    cmd.Parameters.AddWithValue("@role", user.UserRole = "User"); // ne mogu dinamicno jer ako implementiram autentikaciju, samo logirani ce imati pristup, sta nema smisla
                    cmd.Parameters.AddWithValue("@createdby", user.CreatedBy = "Self"); // isto i ovdje, nema veze ajd, uostalom zasto bi registrirani user ili cak administrator pravili nove usere, nema zasto
                    cmd.Parameters.AddWithValue("@updatedby", user.UpdatedBy = "n/a");
                    cmd.Parameters.AddWithValue("@timecreated", user.DateCreated = DateTime.Now);
                    cmd.Parameters.AddWithValue("@timeupdated", user.DateUpdated = DateTime.Now);
                    // u Postmanu ostaviti samo username i password jer sam ovdje sve ostalo zadao

                    theConnection.Open();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return (true, "Added this new user!");
                    }
                    else
                    {
                        return (false, "Failed to create new user.");
                    }
                }                       
            }
            catch (Exception)
            {
                return (false, "Exception");
            }
        }
        //------------------ EDIT USER ---------------------
        public async Task<(RegisteredUser, string)> EditUserAsync(RegisteredUser user, Guid id)
        {
            string authenticatedUser = System.Web.HttpContext.Current.User.Identity.Name;
                        
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmdGet = new SqlCommand("SELECT * FROM RegisteredUser WHERE Id = @id", theConnection);
                    cmdGet.Parameters.AddWithValue("@id", id);
                    theConnection.Open();

                    SqlDataReader reader = await cmdGet.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Close(); // ne treba nam reader vise

                        //---- dohvati rolu --------
                        string role = null;
                        ClaimsPrincipal principal = System.Web.HttpContext.Current.User as ClaimsPrincipal;
                        if (principal != null)
                        {
                            Claim roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                            if (roleClaim != null)
                            {
                                role = roleClaim.Value;
                            }
                        }
                        //-----------------------------

                        if (user.Username == authenticatedUser || role == "Administrator")
                        {
                            SqlCommand cmd = new SqlCommand("UPDATE RegisteredUser SET Username = @username, Pass = @password, UserRole = @role, UpdatedBy = @updatedby, DateUpdated = @timeupdated WHERE Id = @id;", theConnection);

                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@username", user.Username);
                            cmd.Parameters.AddWithValue("@password", user.Pass);

                            if (role == "Administrator")
                            { cmd.Parameters.AddWithValue("@role", user.UserRole); }
                            else
                            { cmd.Parameters.AddWithValue("@role", user.UserRole = "User"); }

                            cmd.Parameters.AddWithValue("@updatedby", user.UpdatedBy = authenticatedUser);
                            cmd.Parameters.AddWithValue("@timeupdated", user.DateUpdated = DateTime.Now);
                            // u Postmanu ostaviti samo sto se treba rucno mijenjati

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                return (user, "User info edited! (ignore null values, it's all good)");
                            }
                            else
                            {
                                return (null, "Did not edit user.");
                            }
                        }
                        else
                        {
                            return (null, "You are either editing a user that is not you, or you are not in Administrator role.");
                        }
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
        //-------------- DELETE USER BY ID ---------------------------
        public async Task<(bool, string)> DeleteUserAsync(Guid id)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmdRead = new SqlCommand("SELECT * FROM RegisteredUser WHERE Id = @id", theConnection);
                    cmdRead.Parameters.AddWithValue("@id", id);
                    theConnection.Open();

                    SqlDataReader reader = await cmdRead.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Close(); // reader odradio svoje

                        SqlCommand cmd = new SqlCommand("DELETE FROM RegisteredUser WHERE Id = @id;", theConnection);
                        cmd.Parameters.AddWithValue("@id", id);

                        int affectedRows = await cmd.ExecuteNonQueryAsync(); // Ovo zapravo obrise, ono iznad ne
                        if (affectedRows > 0)
                        {
                            return (true, "The user has been deleted!");
                        }
                        else
                        {
                            return (false, "Failed to delete the user.");
                        }                        
                    }
                    else
                    {
                        return (false, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (false, "Exception");
            }
        }
        //-------------- GET ALL USERS WITH PAGING, SORTING, FILTERING ---------------------------
        public async Task<(List<RegisteredUser>, string)> GetAllWithParamsAsync(SearchParam search, SortParam sort, PageParam page)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    StringBuilder sb = new StringBuilder(); // slazemo SQL string dio po dio
                    SqlCommand cmd = new SqlCommand();

                    if (search != null) // prvo provjerava jel filtriramo sta, pa tek onda sortiramo i idemo po stranicama
                    {
                        sb.Append("SELECT * FROM RegisteredUser WHERE 1=1"); // potrebno da mozemo dalje samo dodati AND

                        //------------ FILTERING -------------------
                        // pretraga po username
                        if (!string.IsNullOrWhiteSpace(search.Username))
                        {
                            sb.Append(" AND Username LIKE @username");
                            cmd.Parameters.AddWithValue("@username", search.Username);
                        }
                        // pretraga po roli (prikazi samo neku rolu)
                        if (!string.IsNullOrWhiteSpace(search.Role))
                        {
                            sb.Append(" AND UserRole LIKE @role");
                            cmd.Parameters.AddWithValue("@role", search.Role);
                        }
                        // pretraga po createdby
                        if (!string.IsNullOrWhiteSpace(search.CreatedBy))
                        {
                            sb.Append(" AND CreatedBy LIKE @CreatedBy");
                            cmd.Parameters.AddWithValue("@CreatedBy", search.CreatedBy);
                        }
                        // pretraga po updatedby
                        if (!string.IsNullOrWhiteSpace(search.UpdatedBy))
                        {
                            sb.Append(" AND UpdatedBy LIKE @UpdatedBy");
                            cmd.Parameters.AddWithValue("@UpdatedBy", search.UpdatedBy);
                        }
                        // pretraga po vremenu kreiranja
                        if (search.CreatedAfter != null && search.CreatedBefore != null) // izmedju dva datuma
                        {
                            sb.Append(" AND DateCreated >= @createdafter AND DateCreated <= @createdbefore");
                            cmd.Parameters.AddWithValue("@createdafter", search.CreatedAfter);
                            cmd.Parameters.AddWithValue("@createdbefore", search.CreatedBefore);
                        }
                        else if (search.CreatedAfter != null)
                        {
                            sb.Append(" AND DateCreated >= @createdafter"); // samo kreirani nakon nekog datuma
                            cmd.Parameters.AddWithValue("@createdafter", search.CreatedAfter);
                        }
                        else if (search.CreatedBefore != null)
                        {
                            sb.Append(" AND DateCreated <= @createdbefore"); // sammo kreirani prije nekog datuma
                            cmd.Parameters.AddWithValue("@createdbefore", search.CreatedBefore);
                        }
                        // pretraga po vremenu updejtanja
                        if (search.UpdatedAfter != null && search.UpdatedBefore != null) 
                        {
                            sb.Append(" AND DateUpdated >= @updatedafter AND DateUpdated <= @updatedbefore");
                            cmd.Parameters.AddWithValue("@updatedafter", search.UpdatedAfter);
                            cmd.Parameters.AddWithValue("@updatedbefore", search.UpdatedBefore);
                        }
                        else if (search.UpdatedAfter != null)
                        {
                            sb.Append(" AND DateUpdated >= @updatedafter"); // samo kreirani nakon nekog datuma
                            cmd.Parameters.AddWithValue("@updatedafter", search.UpdatedAfter);
                        }
                        else if (search.UpdatedBefore != null)
                        {
                            sb.Append(" AND DateUpdated <= @updatedbefore"); // samo kreirani prije nekog datuma
                            cmd.Parameters.AddWithValue("@updatedbefore", search.UpdatedBefore);
                        }

                        //---------------- SORTING -----------------
                        if (sort.OrderByWhat != null && sort.SortDirection != null) // nema one @nesto kao iznad jer SQL drugacije tumaci za ovo, ovo je kao sigurnosni propust al ajd
                        {
                            sb.Append($" ORDER BY {sort.OrderByWhat} {sort.SortDirection}"); // npr ORDER BY Username ASC/DESC                            
                        }
                        else if (sort.OrderByWhat != null) // za ako mu nisam zadao ASC/DESC
                        {
                            sb.Append($" ORDER BY {sort.OrderByWhat} ASC");
                        }
                        else if (sort.SortDirection != null) // ako mu nisam zadao kolumnu za sorting ali jesam ASC/DESC
                        {
                            sb.Append($" ORDER BY DateCreated {sort.SortDirection}");
                        }

                        //---------------- PAGING ----------------------
                        if (sort.OrderByWhat == null && sort.SortDirection == null) // OFFSET ne radi bez ORDER BY, tako da mu ga moramo zadati ako ne sortiram
                        {
                            sb.Append($" ORDER BY DateCreated DESC");                        
                        }
                        if (page.PageNumber != null && page.EntriesPerPage != null)
                        {
                            sb.Append(" OFFSET @Offset ROWS FETCH NEXT @EntriesPerPage ROWS ONLY;");
                            int? pageOffset = (page.PageNumber - 1) * page.EntriesPerPage;
                            cmd.Parameters.AddWithValue("@Offset", pageOffset);
                            cmd.Parameters.AddWithValue("@EntriesPerPage", page.EntriesPerPage);
                        }
                        else if (page.PageNumber != null) // zadana stranica ali ne broj rezultata po stranici
                        {
                            sb.Append(" OFFSET @Offset ROWS FETCH NEXT 5 ROWS ONLY;"); // default ako mu nismo zadali, ručno podesiti i u SQL i u formuli
                            int? pageOffset = (page.PageNumber - 1) * 5;
                            cmd.Parameters.AddWithValue("@Offset", pageOffset);
                        }
                        else if (page.EntriesPerPage != null) // nije zadana stranica ali je broj rezultata
                        {
                            sb.Append(" OFFSET 0 ROWS FETCH NEXT @EntriesPerPage ROWS ONLY;");
                            cmd.Parameters.AddWithValue("@EntriesPerPage", page.EntriesPerPage);
                        }
                        //-----------------------------------------------------------
                    }
                    else
                    {
                        sb.Append("SELECT * FROM RegisteredUser"); // default
                    };
                    cmd.Connection = theConnection;
                    cmd.CommandText = sb.ToString(); // zada kompletiranu SQL komandu

                    //----- čitanje liste s readerom ------
                    theConnection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<RegisteredUser> list = new List<RegisteredUser>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            RegisteredUser user = new RegisteredUser();

                            user.Id = reader.GetGuid(0);
                            user.Username = reader.GetString(1);
                            user.Pass = reader.GetString(2);
                            user.UserRole = reader.GetString(3);
                            user.CreatedBy = reader.GetString(4);
                            user.UpdatedBy = reader.GetString(5);
                            user.DateCreated = reader.GetDateTime(6);
                            user.DateUpdated = reader.GetDateTime(7); // redoslijed zamjenio

                            list.Add(user);
                        }
                        reader.Close();
                        return (list, "Success");
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
        //-------------- FIND USER FOR AUTHENTICATION ---------------------------
        public async Task<(RegisteredUser, string)> FindUserAsync(string username, string password)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM RegisteredUser WHERE Username = @username AND Pass = @password", theConnection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    theConnection.Open();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        RegisteredUser user = new RegisteredUser();

                        while (reader.Read())
                        {
                            user.Id = reader.GetGuid(0);
                            user.Username = reader.GetString(1);
                            user.Pass = reader.GetString(2);
                            user.UserRole = reader.GetString(3);
                            user.CreatedBy = reader.GetString(4);
                            user.UpdatedBy = reader.GetString(5);
                            user.DateCreated = reader.GetDateTime(6);
                            user.DateUpdated = reader.GetDateTime(7);
                        }
                        reader.Close();
                        return (user, "Success");
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
    }
}
