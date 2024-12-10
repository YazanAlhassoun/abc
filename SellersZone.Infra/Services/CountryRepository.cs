using Microsoft.EntityFrameworkCore;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Wrapper;

namespace SellersZone.Infra.Services
{
    public class CountryRepository : ICountryRepository
    {
        private readonly StoreContext _db;
        public CountryRepository(StoreContext db)
        {
            _db = db;
        }

        public Response<CountryDto> GetCountry(RequestParam param)
            => param.Id == null ? GetCountries() : GetCountryById(param);

        public Response<CountryDto> GetCountries()
        {
            try
            {
                IQueryable<Country> query = _db.Countries;
                var countryDto = query
                     .OrderByDescending(country => country.IsActive)
                     .ThenByDescending(country => country.Ordering)
                     .Select(country => new CountryDto
                     {
                         Id = country.Id,
                         Name = string.IsNullOrEmpty(country.Name) ? country.NameAr : country.Name,
                         NameAr = country.NameAr,
                         Code = country.Code,
                         ImageUrl = country.ImageUrl,
                         IsActive = country.IsActive,
                         IsDefault = country.IsDefault,
                         Ordering = country.Ordering,
                         CreationDate = country.CreatedAt,
                         ModificationDate = country.UpdatedAt
                     }).ToList();

                return new Response<CountryDto>
                {
                    Data = countryDto ?? new List<CountryDto>(),
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<CountryDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

        public Response<CountryDto> GetCountryById(RequestParam param)
        {
            try
            {
                if (param.Id == null)
                {
                    throw new ArgumentNullException(string.Empty, "Country id can't be null");
                }

                IQueryable<Country> query = _db.Countries.Where(s => s.Id == param.Id).Include(s => s.States);

                if (query.Any())
                {
                    var countryDto = query.Select(country => new CountryDto
                    {
                        Id = country.Id,
                        Name = string.IsNullOrEmpty(country.Name) ? country.NameAr : country.Name,
                        NameAr = country.NameAr,
                        Code = country.Code,
                        ImageUrl = country.ImageUrl,
                        IsActive = country.IsActive,
                        IsDefault = country.IsDefault,
                        Ordering = country.Ordering,
                        CreationDate = country.CreatedAt,
                        ModificationDate = country.UpdatedAt,
                        States = country.States.Select(state => new StateDto
                        {
                            Id = state.Id,
                            Name = string.IsNullOrEmpty(state.Name) ? state.NameAr : state.Name,
                            NameAr = state.NameAr,
                            Description = string.IsNullOrEmpty(state.Description) ? state.DescriptionAr : state.Description,
                            DescriptionAr = state.DescriptionAr,
                            IsActive = state.IsActive,
                            Price = state.Price,
                            Ordering = state.Ordering,
                            CountryNameAr = state.Country != null ? state.Country.NameAr : "No Country"
                        }).ToList()
                    }).FirstOrDefault();

                    return new Response<CountryDto>
                    {
                        Data = new List<CountryDto> { countryDto ?? new CountryDto() },
                        IsSuccess = true,
                        StatusCode = 200
                    };
                }
                else
                {
                    return new Response<CountryDto>
                    {
                        StatusCode = 404,
                        ErrorMessage = $"There is no data for country with id '{param.Id}'",
                        IsError = true
                    };
                }
            }
            catch (Exception ex)
            {
                var res = new Response<CountryDto>();

                if (ex is ArgumentNullException argEx)
                {
                    param.ErrorMessage = argEx.Message;
                    param.IsError = true;
                    param.StatusCode = 400;
                }
                else
                {
                    param.ErrorMessage = ex.Message;
                    param.IsError = true;
                    param.StatusCode = 500;
                }
                return res;
            }
        }

        public Response<CountryDto> AddCountry(CountryDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.NameAr) || dto.ImageUrl == null)
                {
                    throw new ArgumentNullException(string.Empty, "Please fill all required field.");
                }
                else
                {
                    var defualtCountry = checkIfThereDefaultCountry(dto);
                    if (string.IsNullOrEmpty(defualtCountry))
                    {
                        // save the image
                        //string image = _fileStorageService.SaveFile(containerName, dto.Image, _httpContextAccessor).GetAwaiter().GetResult();
                        var country = new Country
                        {
                            Name = string.IsNullOrEmpty(dto.Name) ? dto.NameAr : dto.Name,
                            NameAr = dto.NameAr,
                            Code = dto.Code,
                            ImageUrl = dto.ImageUrl,
                            IsActive = dto.IsActive,
                            IsDefault = dto.IsDefault,
                            Ordering = dto.Ordering,
                            CreatedAt = DateTime.Now
                        };

                        _db.Countries.Add(country);
                        _db.SaveChanges();

                        var countryDto = new CountryDto
                        {
                            Id = country.Id,
                            Name = country.Name,
                            NameAr = country.NameAr,
                            Code = country.Code,
                            ImageUrl = country.ImageUrl,
                            IsActive = country.IsActive,
                            IsDefault = country.IsDefault,
                            Ordering = country.Ordering,
                            CreationDate = country.CreatedAt
                        };

                        return new Response<CountryDto>
                        {
                            Data = new List<CountryDto> { countryDto },
                            IsSuccess = true,
                            StatusCode = 200
                        };
                    }
                    else
                    {
                        return new Response<CountryDto>
                        {
                            StatusCode = 404,
                            ErrorMessage = $"The Country with name {defualtCountry} is a defualt",
                            IsError = true
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                var res = new Response<CountryDto>();
                if (ex is ArgumentNullException argEx)
                {
                    res.ErrorMessage = argEx.Message;
                    res.StatusCode = 400;
                }
                else
                {
                    res.ErrorMessage = ex.Message;
                    res.StatusCode = 500;
                }
                res.IsError = true;
                return res;
            }
        }

        public Response<CountryDto> UpdateCountry(CountryDto dto)
        {
            try
            {
                if (dto?.Id == null || dto.Id == 0 || string.IsNullOrEmpty(dto.NameAr))
                {
                    throw new ArgumentNullException(string.Empty, "Please check the entry field.");
                }
                else
                {
                    var defualtCountry = checkIfThereDefaultCountry(dto);
                    if (string.IsNullOrEmpty(defualtCountry))
                    {
                        var oldCountry = _db.Countries.Where(s => s.Id == dto.Id).FirstOrDefault();

                        if (oldCountry != null)
                        {
                            oldCountry.Id = dto.Id;
                            oldCountry.Name = dto.Name;
                            oldCountry.NameAr = dto.NameAr;
                            oldCountry.Code = dto.Code;
                            oldCountry.IsActive = dto.IsActive;
                            oldCountry.IsDefault = dto.IsDefault;
                            oldCountry.Ordering = dto.Ordering;
                            oldCountry.UpdatedAt = DateTime.Now;
                            oldCountry.ImageUrl = dto.ImageUrl;
                            
                            // Update states
                            if (dto.States != null)
                            {
                                var existingStates = _db.States.Where(s => s.CountryId == dto.Id).ToList();

                                // Identify states to be added, updated, and removed
                                var statesToAdd = dto.States.Where(s => !existingStates.Any(es => es.Id == s.Id)).ToList();
                                var statesToUpdate = dto.States.Where(s => existingStates.Any(es => es.Id == s.Id)).ToList();
                                var statesToRemove = existingStates.Where(es => !dto.States.Any(s => s.Id == es.Id)).ToList();

                                // Add new states
                                var listStates = new List<State>();
                                foreach (var state in statesToAdd)
                                {
                                    var newState = new State
                                    {
                                        Name = state.Name,
                                        NameAr = state.NameAr,
                                        Description = state.Description,
                                        DescriptionAr = state.DescriptionAr,
                                        IsActive = state.IsActive,
                                        Price = state.Price,
                                        Ordering = state.Ordering,
                                        CountryId = dto.Id,
                                    };
                                    listStates.Add(newState);
                                }

                                // Update existing states
                                foreach (var state in statesToUpdate)
                                {
                                    var existingState = existingStates.First(es => es.Id == state.Id);
                                    existingState.Name = state.Name;
                                    existingState.NameAr = state.NameAr;
                                    existingState.Description = state.Description;
                                    existingState.DescriptionAr = state.DescriptionAr;
                                    existingState.IsActive = state.IsActive;
                                    existingState.Price = state.Price;
                                    existingState.Ordering = state.Ordering;
                                }

                                _db.States.AddRange(listStates);
                                _db.States.RemoveRange(statesToRemove);
                            }

                            _db.SaveChanges();

                            return new Response<CountryDto>
                            {
                                IsSuccess = true
                            };
                        }
                        else
                        {
                            return new Response<CountryDto>
                            {
                                ErrorMessage = $"Country with id '{dto.Id}' not found.",
                                IsError = true
                            };
                        }
                    }
                    else
                    {
                        return new Response<CountryDto>
                        {
                            ErrorMessage = $"The Country with name {defualtCountry} is a defualt",
                            IsError = true
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                var res = new Response<CountryDto>();
                if (ex is ArgumentNullException argEx)
                {
                    res.ErrorMessage = argEx.Message;
                    res.StatusCode = 400;
                }
                else
                {
                    res.ErrorMessage = ex.Message;
                    res.StatusCode = 500;
                }
                res.IsError = true;
                return res;
            }
        }

        private string checkIfThereDefaultCountry(CountryDto dto)
        {
            var defaultCountry = _db.Countries.FirstOrDefault(c => c.IsDefault);
            if (defaultCountry != null && defaultCountry.Id != dto.Id)
            {
                return defaultCountry.NameAr;
            }
            return string.Empty;
        }

    }
}
