/*!
* Common class
* This class is uesd to implement common javascript functionality 
*/
/// <reference path="jquery-1.4.4-vsdoc.js" />
/// <reference path="jquery.validate-vsdoc.js" />
/// <reference path="jquery.validate.unobtrusive.js" />

//a string contains function
String.prototype.contains = function (it) { return this.indexOf(it) != -1; };

String.prototype.isNullOrWhiteSpace = function (str) {   
    if (typeof str === "string") {
        var isNullOrWhiteSpace = false;

        // Check for null string
        if (str == null || str === "undefined") {
            isNullOrWhiteSpace = true;
        }

        // Check for string with whitespace
        if (str.replace(/\s/g, '').length < 1) {
            isNullOrWhiteSpace = true;
        }

        return isNullOrWhiteSpace;
    }

    if (typeof str === "undefined") {
        return true;
    }
};