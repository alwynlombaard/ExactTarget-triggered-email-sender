ExactTarget triggered email sender
==================================

This library simplifies the sending of ExactTarget "Triggered sends". 

Intended users require access to ExactTarget with a "Triggered Send" configured
and running with an "External Key". 

It also handles "Triggered Sends" with "Data Extensions"

How to use
----------


Get it from Nuget:
```
PM> Install-Package ExactTarget.TriggerEmailSender
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

Trigger the email

```C#
emailTrigger.Trigger(triggeredEmail);
```


