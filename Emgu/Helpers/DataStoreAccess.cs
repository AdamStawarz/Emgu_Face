using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Emgu.Models;

namespace Project_FaceRecognition
{
    class DataStoreAccess:IDataStoreAccess
    {
       
        public String SaveFace(string username, Byte[] faceBlob)
        {
            try
            {
                var exisitingUserId = GetUserId(username);
                if (exisitingUserId == 0) exisitingUserId = GenerateUserId();

                using (var db = new EmguDBContext())
                {
                    var face = new Faces() { username = username, userid = exisitingUserId, faceSample = faceBlob };
                    db.Faces.Add(face);
                    db.SaveChanges();
                }
                //_sqLiteConnection.Open();
                //var insertQuery = "INSERT INTO faces(username, faceSample, userId) VALUES(@username,@faceSample,@userId)";
                //var cmd = new SQLiteCommand(insertQuery, _sqLiteConnection);
                //cmd.Parameters.AddWithValue("username", username);
                //cmd.Parameters.AddWithValue("userId", exisitingUserId);
                //cmd.Parameters.Add("faceSample", DbType.Binary, faceBlob.Length).Value = faceBlob;
                //var result = cmd.ExecuteNonQuery();
                return String.Format("face(s) saved successfully");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }           

        }

        public List<Face> CallFaces(string username)
        {
            var faces = new List<Face>();
            try
            {

                if (username != "ALL_USERS")
                {
                    using (var db = new EmguDBContext())
                    {
                        var _faces = db.Faces.Where(o => o.username == username).ToList();
                        foreach (var _face in _faces)
                        {
                            var face = new Face
                            {
                                Image = _face.faceSample,
                                Id = Convert.ToInt32(_face.id),
                                Label = _face.username,
                                UserId = Convert.ToInt32(_face.userid)
                            };
                            faces.Add(face);
                        }
                        faces = faces.OrderBy(f => f.Id).ToList();
                    }
                }
                else
                {
                    using (var db = new EmguDBContext())
                    {
                        var _faces = db.Faces.Where(o => o.username != null).ToList();
                        foreach (var _face in _faces)
                        {
                            var face = new Face
                            {
                                Image = _face.faceSample,
                                Id = Convert.ToInt32(_face.id),
                                Label = _face.username,
                                UserId = Convert.ToInt32(_face.userid)
                            };
                            faces.Add(face);
                        }
                        faces = faces.OrderBy(f => f.Id).ToList();
                    }
                }
                
               //_sqLiteConnection.Open();
               // var query = username.ToLower().Equals("ALL_USERS".ToLower()) ? "SELECT * FROM faces" : "SELECT * FROM faces WHERE username=@username";
               // var cmd = new SQLiteCommand(query, _sqLiteConnection);
               // if (!username.ToLower().Equals("ALL_USERS".ToLower())) cmd.Parameters.AddWithValue("username", username);
               // var result = cmd.ExecuteReader();
               // if (!result.HasRows) return null;
                
               // while (result.Read())
               // {
               //     var face = new Face
               //     {
               //         Image = (byte[]) result["faceSample"],
               //         Id = Convert.ToInt32(result["id"]),
               //         Label = (String) result["username"],
               //         UserId = Convert.ToInt32(result["userId"])
               //     };
               //     faces.Add(face);
               // }
               // faces = faces.OrderBy(f => f.Id).ToList();      
            }
            catch (Exception ex)
            {
                return null;
            }           
            return faces;
        }


        public int GetUserId(string username)
        {
            var userId = 0;
            try
            {
                using(var db = new EmguDBContext())
                {
                    var ID = db.Faces.Where(o => o.username == username).Select(o => o.userid).FirstOrDefault();
                    userId = Convert.ToInt32(ID);
                }
                
                //_sqLiteConnection.Open();
                //var selectQuery = "SELECT userId FROM faces WHERE username=@username LIMIT 1";
                //var cmd = new SQLiteCommand(selectQuery, _sqLiteConnection);
                //cmd.Parameters.AddWithValue("username", username);
                //var result = cmd.ExecuteReader();
                //if (!result.HasRows) return 0;
                //while (result.Read())
                //{
                //    userId = Convert.ToInt32(result["userId"]);
                    
                //}
            }
            catch
            {
                return userId;
            }           
            return userId; 
        }

        public string GetUsername(int userId)
        {
            var username = "";
            try
            {
                using (var db = new EmguDBContext())
                {
                    username = db.Faces.Where(o => o.userid == userId).Select(o => o.username).FirstOrDefault();                   
                }
                
                //_sqLiteConnection.Open();
                //var selectQuery = "SELECT username FROM faces WHERE userId=@userId LIMIT 1";
                //var cmd = new SQLiteCommand(selectQuery, _sqLiteConnection);
                //cmd.Parameters.AddWithValue("userId", userId);
                //var result = cmd.ExecuteReader();
                //if (!result.HasRows) return username;
                //while (result.Read())
                //{
                //    username = (String)result["username"];

                //}
            }
            catch
            {
                return username;
            }
            
            return username;
        }

        public List<string> GetAllUsernames()
        {
            var usernames = new List<string>();
            try
            {

                using (var db = new EmguDBContext())
                {
                    var _usernames = db.Faces.Where(o => o.username != null).SelectMany(o => o.username).ToList();

                    foreach(var user in _usernames)
                    {
                        usernames.Add(user.ToString());
                    }
                    usernames.Sort();
                }


                //_sqLiteConnection.Open();
                //var query =  "SELECT DISTINCT username FROM faces";
                //var cmd = new SQLiteCommand(query, _sqLiteConnection);
                //var result = cmd.ExecuteReader();
                //while (result.Read())
                //{
                //    usernames.Add((String)result["username"]);
                //}
                //usernames.Sort();
            }
            catch (Exception ex)
            {
                return null;
            }            
            return usernames;
        }


        public bool DeleteUser(string username)
        {
            var toReturn = false;
            try
            {
                using (var db = new EmguDBContext())
                {
                    var face = db.Faces.Where(o => o.username == username).FirstOrDefault();
                    db.Faces.Remove(face);
                    db.SaveChanges();
                    toReturn = true;
                }
                
                //_sqLiteConnection.Open();
                //var query = "DELETE FROM faces WHERE username=@username";
                //var cmd = new SQLiteCommand(query, _sqLiteConnection);
                //cmd.Parameters.AddWithValue("username", username);
                //var result = cmd.ExecuteNonQuery();
                //if (result > 0) toReturn = true;
            }
            catch (Exception ex)
            {
                return toReturn;
            }
           
            return toReturn;
        }

        public int GenerateUserId()
        {
            var date = DateTime.Now.ToString("MMddHHmmss");
            return Convert.ToInt32(date);
        }

        public bool IsUsernameValid(string username)
        {
            throw new NotImplementedException();
        }

        public string SaveAdmin(string username, string password)
        {
            throw new NotImplementedException();
        }
        
    }
}
