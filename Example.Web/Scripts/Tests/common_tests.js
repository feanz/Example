
// common_tests.js
module("Common base library");
test("string contains returns true when string contains search element", function () {
    var testString = "Dave Smith";    
    equals(testString.contains("Dave"),true , "Test string should contain dave");
});
test("is Null Or White Space should return true for white space", function () {
    var testString = " ";
    equals(testString.isNullOrWhiteSpace(),true,"Test string is not null or white space");
});