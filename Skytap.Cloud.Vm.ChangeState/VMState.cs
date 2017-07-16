using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skytap.Cloud.Vm.ChangeState
{
    public class VmState
    {
       

        readonly string _v1Baseurl;
        readonly string _v2Baseurl;
        Dictionary<string, JToken> _status;
        public VmState(string baseUrl,string configId)
        {
             _v1Baseurl = baseUrl + "/configurations/" + configId;
            _v2Baseurl = baseUrl + "/v2/configurations/" + configId;
            
         }
        public string Run(string userName, string password, string vm)
        {
            _status = null;
            NullCheck(userName, password, vm);
            string currnetStatus = CheckStatus(userName, password, vm).ToLower();
            if (currnetStatus.ToLower().Contains("busy"))
            {
                return $"Right now VM: {vm} in {currnetStatus} state so can't change Run";
            }

            string runurl = _v1Baseurl + "/vms/" + vm + "?runstate=running";
            CommonRestApiCall.GetToken(runurl, userName, password, "Put");
            return ExpectedstatusStatus(userName, password, vm, "running");
        }
     
        public string Suspend(string userName, string password, string vm)
        {
            _status = null;
            NullCheck(userName, password, vm);
            string currnetStatus = CheckStatus(userName, password, vm).ToLower();
            if (currnetStatus.ToLower().Contains("busy") || currnetStatus.ToLower().Contains("stopped"))
            {
                return $"Right now VM: {vm} in {currnetStatus} state  can't change Suspend";
            }
            string runurl = $"{_v1Baseurl}/vms/{vm}?runstate=suspended";
            CommonRestApiCall.GetToken(runurl, userName, password, "Put");
            return ExpectedstatusStatus(userName, password, vm, "suspended");
        }

       public string Resume(string userName, string password, string vm)
        {
            _status = null;
            NullCheck(userName, password, vm);
            string currnetStatus = CheckStatus(userName, password, vm).ToLower();
            if (currnetStatus.ToLower().Contains("busy"))
            {
                return $"Right now VM: {vm} in {currnetStatus} state so can't change Resume";
            }
            string runurl = _v1Baseurl + "/vms/" + vm + "?runstate=running";
            CommonRestApiCall.GetToken(runurl, userName, password, "Put");
            return ExpectedstatusStatus(userName, password, vm, "running");
        }
        public string SwitchOff(string userName, string password, string vm)
        {
            _status = null;
            NullCheck(userName, password, vm);
            string currnetStatus = CheckStatus(userName, password, vm).ToLower();
            if (currnetStatus.ToLower().Contains("busy") || currnetStatus.ToLower().Contains("suspended"))
            {
                return $"Right now VM: {vm} in {currnetStatus} state so can't change SwitchOff";
            }
            string runurl = _v1Baseurl + "/vms/" + vm + "?runstate=stopped";
            CommonRestApiCall.GetToken(runurl, userName, password, "Put");
            return ExpectedstatusStatus(userName, password, vm, "stopped");
        }

        private string CheckStatus(string userName, string password, string vm)
        {
            _status = null;
            var statusUrl = _v2Baseurl + "?env_details_meta_info=1&vm_id=" + vm;
            _status = CommonRestApiCall.GetToken(statusUrl, userName, password, "get");
           
            return _status["runstate"].ToString();
        }

        private static void NullCheck(string userName, string password, string vm)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (vm == null) throw new ArgumentNullException(nameof(vm));
        }
        private string ExpectedstatusStatus(string userName, string password, string vm, string expectedstatus)
        {

            string message;
            int retry = 0;
            _status = null;
            string statusUrl = _v1Baseurl + "/vms/" + vm;
            _status = CommonRestApiCall.GetToken(statusUrl, userName, password, "get");
            while (_status != null && (_status["runstate"].ToString() != expectedstatus && retry <40))
            {
                _status = CommonRestApiCall.GetToken(statusUrl, userName, password, "get");
                if(_status["runstate"].ToString()!=expectedstatus && !_status.ContainsKey("errors"))
                Thread.Sleep(1000);
                else
                {
                    break;
                }
                retry++;
            }

            if (_status != null && _status["runstate"].ToString() == expectedstatus)
            {
                message = "vm : " + vm + " now in " + expectedstatus + "state";
            }
            else 
            {
                statusUrl = _v2Baseurl + "?env_details_meta_info=1&vm_id=" + vm;
                _status = CommonRestApiCall.GetToken(statusUrl, userName, password, "get");
                if (_status?["errors"] != null && _status["errors"].ToString().Length>5)
                {
                    message = _status["errors"].ToString();
                }
                else
                {
                    message = "Retry Multiple time but VM : " + vm + " state not changing to " + expectedstatus;
                }
            }
            return message;
        }

    }
}
