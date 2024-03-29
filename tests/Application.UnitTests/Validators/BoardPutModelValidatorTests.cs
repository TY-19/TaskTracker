﻿using FluentValidation.TestHelper;
using TaskTracker.Application.Models;
using TaskTracker.Application.Validators;

namespace TaskTracker.Application.UnitTests.Validators;
public class BoardPutModelValidatorTests
{
    private readonly BoardPutModelValidator _validator;
    public BoardPutModelValidatorTests()
    {
        _validator = new BoardPutModelValidator();
    }

    [Fact]
    public void ShouldBeValid_IfProvidedWithCorrectData()
    {
        var model = new BoardPutModel() { Name = "Updated" };

        var result = _validator.TestValidate(model);

        Assert.True(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNewNameIsAnEmptyString()
    {
        var model = new BoardPutModel() { Name = string.Empty };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsTooShort()
    {
        var model = new BoardPutModel() { Name = "Aa" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameIsTooLong()
    {
        var model = new BoardPutModel() { Name = "Very very very very very very very very very very very very very long name" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
    [Fact]
    public void ShouldBeInvalid_WhenNameContainsOnlyDigits()
    {
        var model = new BoardPutModel() { Name = "12345" };

        var result = _validator.TestValidate(model);

        Assert.False(result.IsValid);
    }
}
