Lighter.NET is a tiny innovative library to improve the developemnt of asp.net core application including front-end and back-end works. 
This project aims to make more sense to the current asp.net core application development. 
The Lighter.NET is intended to be used together with Lighter.NET.DB nuget package which provides a light-weighted ORM utilizing EntityFramework 6.4.4 but more simplified.

With Lighter.NET, you can:
1. Say Goodbye to ViewData and ViewBag.
2. Say Goodby to the long-winded Entity Data Model file structures, such as *.edmx, *.Context.tt, *.Designer.cs, and unecessary DbSet for db table mapping.
3. Say Goodbye to ever changing HtmlHelper as .NET version rapidly increasing.
4. Say Hello to a more efficient localization mechanism with CompositeLocalizer.
5. Say Goodbye to jQuery.
6. Say Goodbye to Bootstrap.

The main features include:
1. A ViewModelWrapper for delivering multiple models in a strong type manner between controller and view.
2. A set of UiComponents for composing and rendering most frequently used html element in an efficient and consistent coding style across different .net version.
3. To be used together with Lighter.NET.DB nuget package which provides a light-weighted ORM utilizing EntityFramework 6.4.4 but more simplified.
4. A set of javascript library for a light-weighted client side work handling.
5. A light-weight responsive css framework for responsive design.
6. Some other helpers to simplify common application development works.

Release Notes
Ver. 1.4.0
	1. Add $LighterObject.autoComplete() method in lighter.js. The autoComplete() features an onCompleteCallback to provide more sophisticated intercation to the matched option, a mustMatch flag to prevent arbitrary input and a support to synonyms lookup.
	2. Remove ApiResultHelper
	3. Remove DebugMessages from ApiResult
	4. Add private _exceptions field and public AddException() and Exceptions() methods to ApiResult to make it capable deliver exception object out to the caller. In the mean while, avoid sensitive security information to be expose to client while doing json serialization.
	5. Bug fix for ValidationHelper

Ver. 1.3.0
	1. Introduce the $LighterObject to lighter.js for easy client ui element manipulation, such as visibility, style, parent and child element searching and event handling.
	2. Introduce $ElementFlag and flag binding functionality to lighter.js which automatively triggers some ui interactive behavior according to target elements flag state.
	3. Introduce an $StateMachine to lighter.js for general dynamic state management, such as client side table row editing and batch updating.
	4. Introudec an $Map to lighter.js for easy client side dictionary lookup.
	5. Adding ContentConverter property for Comlumn of T UiComponent to enhance table column flexibility, for example convert text to hyperlink.
	6. Adding HideColumnHeader property for TableOfT UiComponent.
	7. Adding CanAdd, CanDelete property for TableOfT, which also enable the data source model list to be combined with the client table object for better client row event handling ability.
	8. Adding Step property for DatatimeLocal UiComponent.
	9. Adding IsDisabled property for IUiElement.
	10. Adding GetValue() extension method for all Class type which enables getting the property value by string of property name.
	11. Fixing Column width problem for AutoSerialNo and RowCommnads columns.
	12. Enhancement to lighter.css for better rwd effects and adding some ui component style like Step Bar Style.

Ver. 1.2.5
	1. New FileHelper.FormatFilesize() to format filesize to GB, MB or KB.
	2. Bug fix for ViewModelWrapper to avoid key conflict.

Ver. 1.2.4
Ver. 1.2.3
	1. Enhancement of ViewModelWrapper for model value validation.
	2. Improvement of UiElements including Date, DateTimeLocal and Label.
	3. New UiElement of Month for year and month input.
	4. Improvement of ValidationHelper.

Ver. 1.2.2
	1. Add StateModel for change tracking of data record
	2. Enhancement of LogModel and HttpRequestErrorLogModel for more reasonable and efficient logging information.
	3. TableOfT UiComponent now support inline row editing and multiple rows editing and change tracking.
	4. Enhancements for UiComponents including ColumnOfT, UiElementBase, Column, CheckBox, Date, DateTimeLocal, Number, Select, Text,TextArea.

Ver. 1.2.1
	1. Resolving conflicked files.

Ver. 1.2.0
	1. Reconstruct the ViewModelWrapper to inherit from BaseViewModel which can be used to communicate between View and Layout.
	2. New PrivacyHelper for privacy protection functions.
	3. New PercentWidth for defining width which support basic adding and substraction operation.

Ver. 1.1.0
	1. Adding CompositeLocalizer which combining multiple localizers for DataAnnotation localization.
	2. Adding  javascript libraries and css to facilitate font-end responsive web development.

Ver. 1.0.0
	First Release.