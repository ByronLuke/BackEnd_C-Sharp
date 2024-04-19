using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class FriendV3Service : IFriendV3Service
    {
        private IDataProvider _dataProvider = null;
        public FriendV3Service(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }
       
        public List<FriendV3> GetAll()
        {
            List<FriendV3> friendSkillsList = null;
            string procName = "[dbo].[Friends_SelectAllV3]";
            _dataProvider.ExecuteCmd(procName,
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    FriendV3 friendSkill = MapV3Friend(reader);

                    if (friendSkillsList == null)
                    {
                        friendSkillsList = new List<FriendV3>();
                    }

                    friendSkillsList.Add(friendSkill);

                });

            return friendSkillsList;
        }

        public FriendV3 GetById(int id)
        {
            string procName = "[dbo].[Friends_SelectByIdV3]";

            FriendV3 friendV3 = null;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                friendV3 = MapV3Friend(reader);
            });
            return friendV3;
        }

        private static FriendV3 MapV3Friend(IDataReader reader)
        {
            FriendV3 friendSkill = new FriendV3();
            friendSkill.PrimaryImage = new Image();
            friendSkill.Skills = new List<Skill>();

            int startingIndex = 0;

            friendSkill.Id = reader.GetInt32(startingIndex++);
            friendSkill.Title = reader.GetString(startingIndex++);
            friendSkill.Bio = reader.GetString(startingIndex++);
            friendSkill.Summary = reader.GetString(startingIndex++);
            friendSkill.Headline = reader.GetString(startingIndex++);
            friendSkill.Slug = reader.GetString(startingIndex++);
            friendSkill.StatusId = reader.GetInt32(startingIndex++);

            friendSkill.PrimaryImage.Id = reader.GetInt32(startingIndex++);
            friendSkill.PrimaryImage.TypeId = reader.GetInt32(startingIndex++);
            friendSkill.PrimaryImage.Url = reader.GetString(startingIndex++);

            friendSkill.Skills = reader.DeserializeObject<List<Skill>>(startingIndex++);

            friendSkill.UserId = reader.GetInt32(startingIndex++);
            friendSkill.DateCreated = reader.GetDateTime(startingIndex++);
            friendSkill.DateModified = reader.GetDateTime(startingIndex++);
            return friendSkill;
        }
    }
}
