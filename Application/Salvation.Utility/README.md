### TLS 1.3 Issues with Talent Structure Update

When using the TalentStructureUpdate Service, TLS 1.3 has been enabled by the datasource.

If on a machine which can't handle TLS1.3 (Anything below Windows 11 / Server 2022), 
use `GetJsonTalentDataFromFileAsync()` instead of `GetJsonTalentDataAsync()`, 
and manually download and place the .json file in the debug folder so it's accessible on launch.
