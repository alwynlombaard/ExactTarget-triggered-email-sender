using ExactTarget.TriggeredEmail.Trigger;

namespace ExactTarget.TriggeredEmail.Creation
{
    public interface ITriggeredEmailCreator
    {
        /// <summary>
        /// <para>Creates a Triggered Send Definition with an email template containing a content area and replacement value %Subject%.</para>
        /// <para>Use this if you want to edit the email markup in the ET UI.  </para>
        /// </summary>
        /// <param name="externalKey"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        int Create(string externalKey, Priority priority = Priority.Medium);

        /// <summary>
        /// <para>Creates a Triggered Send Definition with an email template containing the supplied mark up. (and %%Subject%% replacement value)</para>
        /// <para>Use this if you do NOT wish to log in to ET UI to edit the email markup.</para>
        /// <para>Replacement values in layoutHtml (for example %%FirstName%% or %%myOwnVariableName%% ) 
        /// will be parsed and created as fields in the Data Extension.</para>
        /// </summary>
        /// <param name="externalKey"></param>
        /// <param name="layoutHtml">Replacement values in layoutHtml (for example %%FirstName%% or %%MyOwnVariableName%% ) 
        /// will be parsed and corresponding fields created in the Data Extension with that name.</param>
        /// <param name="priority"></param>
        /// <returns></returns>
        int Create(string externalKey, string layoutHtml, Priority priority = Priority.Medium);

        /// <summary>
        /// <para>Creates a Triggered Send Definition with an email template containing a content area. (and %%Subject%% replacement value)</para>
        /// <para>You can specify html layouts for above and below the body tags.</para>
        /// </summary>
        /// <param name="externalKey"></param>
        /// <param name="layoutHtmlAboveBodyTag"></param>
        /// <param name="layoutHtmlBelowBodyTag"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        int CreateTriggeredSendDefinitionWithEmailTemplate(string externalKey, string layoutHtmlAboveBodyTag, string layoutHtmlBelowBodyTag, Priority priority = Priority.Medium);

        /// <summary>
        /// <para>Creates a "Paste HTML" Triggered Send Definition with "Subject", "Body" and "Head" replacement value names. </para>
        /// </summary>
        /// <param name="externalKey"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        int CreateTriggeredSendDefinitionWithPasteHtml(string externalKey, Priority priority = Priority.Medium);

        /// <summary>
        /// <para>Creates a "Paste HTML" Triggered Send Definition with "Subject" replacement value and  using layoutHtml. </para>
        /// <para>Replacement values in layoutHtml (for example %%FirstName%% or %%myOwnVariableName%% ) 
        /// will be parsed and created as fields in the Data Extension.</para>
        /// </summary>
        /// <param name="externalKey"></param>
        /// <param name="layoutHtml"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        int CreateTriggeredSendDefinitionWithPasteHtml(string externalKey, string layoutHtml, Priority priority = Priority.Medium);

        void StartTriggeredSend(string externalKey);
    }
}