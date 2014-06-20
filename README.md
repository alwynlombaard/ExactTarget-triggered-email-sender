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
//the email to trigger
 var triggeredEmail = new ExactTargetTriggeredEmail("external-key-of-trigger", 
                                "recipient@uri.test" );

//specify values for the Data Extension if any (optional)
triggeredEmail.AddReplacementValue("DataExtensionFieldName1", "Value 1");
triggeredEmail.AddReplacementValue("DataExtensionFieldName2", "Value 2");

//trigger the email
var emailTrigger = new EmailTrigger(config);
emailTrigger.Trigger(triggeredEmail);
```

Creating a new Triggered Send Definition
-----------------------------

In only 3 lines of code you can create a TriggeredSendDefinition
in ExactTarget that you can then use to send an unstyled HTML email (with tracking)
to a recipient. When triggering an email you only need to supply 
recipient address, subject, and html body content.


```C#
//create and start Triggered Send (only required to do this once)
var triggeredEmailCreator = new TriggeredEmailCreator(config);

triggeredEmailCreator.Create("my-new-external-key-of-trigger");

triggeredEmailCreator.StartTriggeredSend("my-new-external-key-of-trigger");

```
* The above example will create a Triggered Send Definition,
Data Extension with "Subject" and "Body",
Paste HTML Email Template, an Email from that template
and a Delivery Profile for the Data Extension without header and footer
in ExactTarget.

Now you can trigger an email:
```C#
var triggeredEmail = new ExactTargetTriggeredEmail("my-new-external-key-of-trigger", 
										"recipient@temp.uri");
triggeredEmail.AddReplacementValues(new Dictionary<string, string>
	{
		{"Subject","Test email"}, 
		{"Body","<h2>Test email heading</h2><p>Test paragraph</p>"}
	});

var emailTrigger = new EmailTrigger(config);
emailTrigger.Trigger(triggeredEmail);
```
