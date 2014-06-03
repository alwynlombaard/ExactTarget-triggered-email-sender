ExactTarget triggered email sender
==================================

Sends a triggered email via ExactTarget.

How to use
----------


Get it from Nuget:
```
PM> Install-Package ExactTarget.TriggerEmailSender
```

Specify external key (customer key) of the "triggered send" 
and recipient that the email will go to
 
```C#
 var triggeredEmail = new ExactTargetTriggeredEmail("external-key-of-trigger", 
                                "recipient@uri.test" );

```

Specify values for the Data Extension if any (optional)

```C#
triggeredEmail.AddReplacementValue("DataExtensionFieldName1", "Value 1");
triggeredEmail.AddReplacementValue("DataExtensionFieldName2", "Value 2");
```

Create a trigger with configuration values

```C#
 var emailTrigger = new EmailTrigger(new ExactTargetConfiguration
            {
                ApiUserName = "API_User",
                ApiPassword = "API_Password",
                //use your endpoint given to you by ET
				EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",
                ClientId = 6269485,//optional business unit id
            });
//above code is only for demonstration purposes
//ideally you would want to load your configuration from a configuration file		
```

Trigger the email

```C#
emailTrigger.Trigger(triggeredEmail);
```


