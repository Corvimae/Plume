# CoreEngine Module Guide

## Creating a Module

A new module can be created by making a directory and placing a `module.json` 
definition file within it. The base level object of the definition file should 
be an object, not an array.

Definition files must contain the following:

**`module` (object)**

The `module` block contains basic info on the module itself.

**name (string) [required]**: The name of the module. 

**version (string) [required]**: The version number of the module. `version` is a string, 
so you may number your versions however you like.

**author (string)**: The name of the module's developer.

An example `module` block might like like this:

    {
      "name": "My Module",
      "version": "0.1.1",
      "author": "AcceptableIce"
    }

## Entities

Game entities in CoreEngine exist in individual Ruby files. Each Ruby file should contain a single class;
these classes can either define new game-world objects (like a new map tile or item), or a new entity type
(a shooter game might include many types of guns).

CoreEngine recursively looks through your module for all `.rb` files, so you can design your module's folder
structure however you like.

**CoreEngine is built on IronRuby, and thus runs Ruby 1.9.2**


### Creating a New Entity

A new entity can be defined by creating a new `.rb` file somewhere in your module's folder. The name of this
file isn't used by CoreEngine, but we recommend giving the file the same name as your class.

All entities must extend the `BaseEntity` class or one of its decendants. CoreEngine comes with the following
entity types predefined:
 
* MapTile

If your entity is going to extend one of CoreEngine's predetermined entity types (or if it is to be a new entity type),
the module `CoreEngine::Entities` must be imported. Usually you will also import `CoreEngine::Scripting`, which 
provides methods for drawing objects to the screen, accessing game data, and more.

Define your new entity by extending one a BaseEntity. In this guide, we'll make a new `MapTile`.

    import CoreEngine::Entities
    import CoreEngine::Scripting
    
    class MyTile < MapTile
    
    end

----
#### The Entity Lifecycle

When the module is initially loaded, CoreEngine records all defined entity models and categorizes them by
type. When this initial compilation and categorization process occurs, CoreEngine runs the `register` method.
By default, this method is empty, although some entity types may extend it (for example, `MapTile` uses
`register` to load its texture into memory.)

If your entity requires loading textures or other operations that should occur only once, your entity should
extend `register`. Extended classes should not attempt to load a shared texture again; the child class
will automatically have references to all textures loaded by the parent class.

IronRuby prevents Ruby's standard `initialize` method from calling its parent constructor. To get around this,
CoreEngine provides a `create` method, which runs whenever an instance of the entity is created. **Your entity
should not override `initialize` or `self.new`; doing so breaks the `register` functionality, as well as any 
constructor functionality of any parent classes.**

CoreEngine does not provide a destructor method.

----

Even with nothing in it, `MyTile` is still a usable tile. By default, a `MapTile` looks for a file named
`[ClassName].png` in `register`, stores it at `self.texture` during `create`, and draws it to the current
map position in `draw`. 

If we wished to extend any of these methods, we can do so simply by declaring them. For example, if we wanted
to print the current X position of each tile on top of it, we might add the following method:

    def draw
      super
      position = self.get_position
      Canvas.draw_string(CoreFont.System, position.x.to_s , position.x * 32, position.y * 32, CoreColor.White)
    end

This method first calls the normal `MapTile` draw method (which simply draws the loaded tile texture at its current
grid position). Then, it retrives the current position. Finally, it draws a string onto the map using one of the
fonts defined in the `CoreFont` class.

#### Creating a New Entity Type

New entity types are created similarly to entities, except they instead extend the `BaseModel` class. Note that
entity types cannot be instanced; they must be extended upon with new entities.

## IronRuby Limitations

#### initialize and Model.new

IronRuby does not allow superconstructors to be called from `initialize`. CoreEngine provides the
`register` and `create` methods to bypass this.

#### require_relative

IronRuby does not support `require_relative`. CoreEngine automatically includes all parent directories
in your module as search directories, so your `require` can just include the filename.

#### Speed Concerns

Reflexive calls to IronRuby methods are inherently slow. CoreEngine attempts to optimize these
calls as much as possible, but it's important to consoldate as much as possible.
Any supplied method which takes a symbol referring to a method as an argument, such as
`draw_on_layer`, will be expensive.
