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

Creating a new Triggered Send Definition with a "Paste HTML" email in ExactTarget
---------------------------------------------------------------------------------

In only 3 lines of code you can create a TriggeredSendDefinition
in ExactTarget that you can then use to send an HTML email (with tracking)
to a recipient. When triggering an email you only need to supply 
recipient address, subject, html body and optionally html head content.

```C#
//create and start Triggered Send (only required to do this once)
var triggeredEmailCreator = new TriggeredEmailCreator(config);

triggeredEmailCreator.CreateTriggeredSendDefinitionWithPasteHtml(
				"new-external-key",
				"<html>" +
				"<head>" +
				"<style>.green{color:green}</style>" +
				"</head>" +
				"<body>Hello %%FirstName%%,   " +
				"<p>This is a paste Html email with custom fields.</p>" +
				"<p class='green'>Green Content: %%MyOwnValue%% ...</p>" +
				"<body>" +
				"<html>");

triggeredEmailCreator.StartTriggeredSend("new-external-key");
```
* The above example will create a Triggered Send Definition,
Data Extension with "Subject" and replacement values you specified in
your layout (%%FirstName%%, %%MyOwnValue%%), Paste HTML Email
and a Delivery Profile for the Data Extension without header and footer
in ExactTarget.

Now you can trigger an email:
```C#
var triggeredEmail = new ExactTargetTriggeredEmail("new-external-key", 
										"recipient@temp.uri");
triggeredEmail.AddReplacementValue("Subject","Test email");
triggeredEmail.AddReplacementValue("FirstName","John");
triggeredEmail.AddReplacementValue("MyOwnValue","Some test copy here...");

var emailTrigger = new EmailTrigger(config);
emailTrigger.Trigger(triggeredEmail);
```

Creating a new Triggered Send Definition with an email template in ExactTarget
------------------------------------------------------------------------------

In only 3 lines of code you can create a TriggeredSendDefinition
in ExactTarget that you can then use to send an HTML email (with tracking) containing placeholder values
to a recipient. When triggering an email you only need to supply 
recipient address, subject, and the replacement values you specified in your layout Html.

```C#
//create and start Triggered Send (only required to do this once)
var triggeredEmailCreator = new TriggeredEmailCreator(config);

triggeredEmailCreator.Create(
						"new-external-key",
						"<html><head><style>.green{color:green}</style></head><body>Hello %%FirstName%%,   <p class='green'>Green Content: %%MyOwnValue%% ...</p><body><html>");

triggeredEmailCreator.StartTriggeredSend("new-external-key");
```
* The above example will create a Triggered Send Definition,
Data Extension with "Subject" and replacement values you specified in
your layout (%%FirstName%%, %%MyOwnValue%%),
Paste HTML Email Template, an Email from that template
and a Delivery Profile for the Data Extension without header and footer
in ExactTarget.

Now you can trigger an email:
```C#
var triggeredEmail = new ExactTargetTriggeredEmail("new-external-key", 
										"recipient@temp.uri");
triggeredEmail.AddReplacementValue("Subject","Test email");
triggeredEmail.AddReplacementValue("FirstName","John");
triggeredEmail.AddReplacementValue("MyOwnValue","Some test copy here...");

var emailTrigger = new EmailTrigger(config);
emailTrigger.Trigger(triggeredEmail);
```
