@CreateNewUser 
Feature: Create a new user
	In order to manage users in my system
	As a site superuser
	I want to be able to create, view and manage user records

Scenario: Create a basic user record
	Given I am Authenticated on the site
	And I on the User admin screen 	
	When I click the "Create New" link
	And I enter the following information
		| Field | Value                           |
		| UserName  | someuser                    |
		| FirstName	| luke						  |		
		| LastName	| skywalker					  |		
		| Email		| luke.skywalker@theforce.com |		
	And I click the "Create" button	
	Then I should be redirected to the User admin screen
		