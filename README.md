ExactTarget triggered email sender
==================================

Sends a triggered email via ExactTarget.

How to use
----------

Specify external key (customer key) of the "triggered send" 
and recipient that the email will go to
 
```C#
 var triggeredEmail = new ExactTargetTriggeredEmail("external-key-of-trigger", 
                                "recipient@uri.test" );

```

Specify values for the Data Extension (optional)

```C#
triggeredEmail.AddReplacementValues(new Dictionary<string, string>
                {
                    {"DataExtensionFieldName1","Value 1"}, 
                    {"DataExtensionFieldName2","Value 2"}
                });
```

Create a trigger with configuration values

```C#
var emailTrigger = new EmailTrigger(config);
```

Trigger the email

```C#
emailTrigger.Trigger(triggeredEmail);
```


An example of configuration object
```C#
//store and load these values from a config file
new ExactTargetConfiguration
{
    ApiUserName = "API_User",
    ApiPassword = "API_Password",
    EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",
    SoapBinding = "ExactTarget.Soap",
    ClientId = 100, // optional  business unit to use
};
```
