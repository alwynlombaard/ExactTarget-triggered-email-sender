ExactTarget triggered email sender
==================================

This library simplifies the creation and triggering of ExactTarget "Triggered Send Definitions" via the ExactTarget API. 

Get it from Nuget:
```
PM> Install-Package ExactTarget.TriggerEmailSender
```

Triggering an email	
-------------------

```C#
//create configuration
var config = new ExactTargetConfiguration
{
	ApiUserName = "API_User",
	ApiPassword = "API_Password",
	//use your endpoint given to you by ET
	EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",
	ClientId = 6269485,//optional business unit id
};
//above code is only for demonstration purposes
//ideally you would want to load your configuration from a configuration file		
```

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
var emailTrigger = new EmailTrigger(config);
emailTrigger.Trigger(triggeredEmail);
```

Creating a new Triggered Send Definition
-----------------------------

The following example will create a Triggered Send Definition,
Data Extension with "Subject" and "Body",
Email Template, an Email from that template
and a Delivery Profile for the Data Extension without header and footer
in ExactTarget.

```C#
//create and start Triggered Send
var triggeredEmailCreator = new TriggeredEmailCreator(config);

triggeredEmailCreator.Create("my-new-external-key-of-trigger");

triggeredEmailCreator.StartTriggeredSend(externalKey);

```

Now you can trigger an email:
```C#
var triggeredEmail = new ExactTargetTriggeredEmail("my-new-external-key-of-trigger", 
										"someone@temp.uri");
triggeredEmail.AddReplacementValues(new Dictionary<string, string>
	{
		{"Subject","Test email"}, 
		{"Body","<h2>Test email heading</h2><p>Test paragraph</p>"}
	});

var emailTrigger = new EmailTrigger(config);
emailTrigger.Trigger(triggeredEmail);
```
