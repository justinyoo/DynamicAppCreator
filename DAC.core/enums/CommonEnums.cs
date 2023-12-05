using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.enums
{

    public enum DatabaseTypesEnum
    {
        Database = 0,
        File = 1
    }

    public enum DatatableTypesEnum
    {
        Table = 0,
        View = 1
    }


    public enum LogTypesEnum
    {
        Created = 0,
        Modified = 1,
        Deleted = 2,
        Locked = 3,
    }

    public enum EncryptionTypesEnum
    {
        none = 0,
        md5 = 1,
        sha = 2,
        certificate = 3,
        shaOnlyStore = 4,
        certificateOnlyStore = 5
    }

    public enum TriggerTypesEnum
    {
        onCreate = 0,
        onDelete = 1,
        onUpdate = 2,
        onListed = 3
    }

    public enum DataTriggerTypesEnum
    {
        onCreate = 0,
        onUpdate = 2
    }

    public enum SelectBehaviors
    {
        /// <summary>
        /// include to the result  if even is specified in the select query.
        /// </summary>
        strict,

        /// <summary> 
        /// include to the result even if  is not specified in the select query.
        /// </summary>
        always,

        /// <summary>
        /// exclude to the result even if  is   specified in the select query.
        /// </summary>
        nothing
    }

    public enum ValueAutomationTypes
    {
        CurrentDate,
        CurrentTime,
        CurrentUserName,
        CurrentUserID,
        CurrentUserEMail,
        CurrentUserGsm,
        CurrentUserPhone,
        CurrentCompanyName,
        CurrentCompanyID,
        CustomValue,
        CustomClaim,

    }
}
