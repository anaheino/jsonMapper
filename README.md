# JsonMapper
Maps CSV Data to specified places in a JSON object and multiplies the JSON with provided values.

This software is a proof of concept implementation of an idea I had.

The purpose is to easily multiply a base template of JSON/XML into numerous JSON/XML -objects with modified values.

This is achieved by using a simple internal macrolanguage, in order to produce and parametrize different types of mapper objects.

Software is provided with a .csv file with multiple different values.
Then, the software reads the headers of each column and forms a list of values.

These lists of values are then mapped into different parts of the template JSON/XML -object.

Currently, there are 5 diffent mapper classes implemented.

## Mapper classes

1. PropertyMap 
   This mapper class performs a simple 1 to 1 mapping of values, as in such column 1 -> property 1.

2. SiblingPropertyMap 
   This is pretty much the same as above, except it maps into a different property on the same level as the parametrized.
   An example use case would be: I only want to map to column B, if column A in the template has value X.

3. PropertyInserter
   This is a same type of map as the first one, except it allows you to insert a string into the value. 
   An example use case would be: map values from header A to templates property B, but insert string "LAA" before value.
   Resulting value would be: property B = "LAA {value from A}"

4. Sibling column map
    This is the same type of mapper as SiblingPropertyMap, except it performs the value comparisons in the provided value list, 
    and not in the template. As an example, you can say "Map values A to property B, but only if values C on the same row has provided value"

5.  PropertyValueExcluder
    This excludes rows from the final result of all performed mappings. You can say things like "I want all values from the .csv-file,
    except the rows in which Column C has parametrized value" 


### Please note, that cmdline version is not yet utilized.
Unfortunately, the software is currently implemented in .NET framework, because I wanted to make a simple UI for testing if this proof of concept would work.
This means no linux for you guys yet.

