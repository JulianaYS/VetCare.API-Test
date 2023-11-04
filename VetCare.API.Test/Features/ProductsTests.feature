Feature: Product Adding
As a Developer
I want to add Product through API
So that it can be available to applications.

    Background: 
        Given the Endpoint https://localhost:7126/api/v1/products is available

    @product-adding
    Scenario: Add Product
        When a Post Request is sent 
          | name     | description         | price | image                         | stock |
          | galletas | galletas para gatos | 13.5  | https://th.bing.com/th/id/OIP | 3     |
        Then A Response is received with Status 200
        And a Product Resource is included in Response Body 
          | id | name     | description         | price | image                         | stock |
          |  1 | galletas | galletas para gatos | 13.5  | https://th.bing.com/th/id/OIP | 3     |
	
    @product-duplicated      
    Scenario: Add Product with existing Name
        Given A Product is already stored
          | id | name     | description         | price | image                         | stock |
          |  1 | galletas | galletas para gatos | 13.5  | https://th.bing.com/th/id/OIP | 3     |
        When a Post Request is sent 
          | name     | description         | price | image                         | stock |
          | galletas | galletas para gatos | 13.5  | https://th.bing.com/th/id/OIP | 3     |
        Then A Response is received with Status 400
        And An Error Message is returned in Response Body, with value "An Product with the same name already exists."