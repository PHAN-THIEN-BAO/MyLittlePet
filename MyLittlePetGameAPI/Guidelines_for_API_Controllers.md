# Guidelines for Updating All API Controllers

Follow these steps to update all your API controllers to avoid serialization issues and provide consistent responses:

## 1. Inherit from BaseApiController

```csharp
public class YourController : BaseApiController
{
    // ...
}
```

## 2. Wrap Methods in Try-Catch Blocks

```csharp
[HttpGet]
public ActionResult<IEnumerable<object>> Get()
{
    try
    {
        // Your code here
        return ApiOk(result);
    }
    catch (Exception ex)
    {
        return ApiServerError<object>(ex);
    }
}
```

## 3. Return DTOs or Anonymous Types Instead of Entity Objects

```csharp
// Instead of returning entities directly:
// return Ok(myEntity);

// Return a DTO or anonymous type:
return ApiOk(new {
    id = myEntity.Id,
    name = myEntity.Name,
    // Only include properties needed by the client
    // Avoid including navigation properties
});
```

## 4. Use the Helper Methods for Standard Responses

- `ApiOk<T>(data, message)` - For successful operations
- `ApiError<T>(message, statusCode)` - For client errors
- `ApiNotFound<T>(message)` - For not found resources
- `ApiServerError<T>(exception, userMessage)` - For server errors

## 5. Common Patterns for Each HTTP Method

### GET - Retrieving Data
```csharp
[HttpGet("{id}")]
public ActionResult<object> GetById(int id)
{
    try
    {
        var entity = _context.Entities.Find(id);
        if (entity == null)
            return ApiNotFound<object>("Resource not found");
            
        return ApiOk(new {
            id = entity.Id,
            // Other properties
        });
    }
    catch (Exception ex)
    {
        return ApiServerError<object>(ex);
    }
}
```

### POST - Creating Data
```csharp
[HttpPost]
public ActionResult<object> Create(YourCreateModel model)
{
    try
    {
        // Validation
        if (!ModelState.IsValid)
            return ApiError<object>("Invalid data provided");
            
        // Create entity
        var entity = new Entity { /* properties */ };
        _context.Entities.Add(entity);
        _context.SaveChanges();
        
        return ApiOk(new {
            id = entity.Id,
            // Other properties
        }, "Resource created successfully");
    }
    catch (Exception ex)
    {
        return ApiServerError<object>(ex);
    }
}
```

### PUT - Updating Data
```csharp
[HttpPut("{id}")]
public ActionResult<object> Update(int id, YourUpdateModel model)
{
    try
    {
        // Find entity
        var entity = _context.Entities.Find(id);
        if (entity == null)
            return ApiNotFound<object>("Resource not found");
            
        // Update entity
        entity.Property = model.Property;
        _context.Entities.Update(entity);
        _context.SaveChanges();
        
        return ApiOk(new {
            id = entity.Id,
            // Other properties
        }, "Resource updated successfully");
    }
    catch (Exception ex)
    {
        return ApiServerError<object>(ex);
    }
}
```

### DELETE - Removing Data
```csharp
[HttpDelete("{id}")]
public ActionResult Delete(int id)
{
    try
    {
        // Find entity
        var entity = _context.Entities.Find(id);
        if (entity == null)
            return ApiNotFound<object>("Resource not found");
            
        // Delete entity
        _context.Entities.Remove(entity);
        _context.SaveChanges();
        
        return ApiOk<object>(null, "Resource deleted successfully");
    }
    catch (Exception ex)
    {
        return ApiServerError<object>(ex);
    }
}
```
