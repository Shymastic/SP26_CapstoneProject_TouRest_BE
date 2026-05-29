using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Service;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderUserRepository _providerUserRepository;
        private readonly IMapper _mapper;
        public ServiceService(IServiceRepository serviceRepository, IProviderRepository providerRepository, IProviderUserRepository providerUserRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _providerRepository = providerRepository;
            _providerUserRepository = providerUserRepository;
            _mapper = mapper;
        }
        public async Task<ServiceDTO?> GetServiceById(Guid id)
        {
            return _mapper.Map<ServiceDTO?>(await _serviceRepository.GetByIdAsync(id));
        }
        public async Task<IEnumerable<ServiceDTO>> GetAllServices()
        {
            return _mapper.Map<IEnumerable<ServiceDTO>>(await _serviceRepository.GetAllAsync());
        }

        public async Task<IEnumerable<ServiceDTO>> GetServicesByProviderId(Guid providerId)
        {
            var provider = await _providerRepository.GetByIdAsync(providerId);
            if (provider == null)
            {
                throw new KeyNotFoundException($"Provider with ID '{providerId}' does not exist.");
            }
            return _mapper.Map<IEnumerable<ServiceDTO>>(await _serviceRepository.GetByProviderIdAsync(providerId));
        }
        public async Task<ServiceDTO> CreateService(ServiceCreateRequest service)
        {
            var provider = await _providerRepository.GetByIdAsync(service.ProviderId);
            if (provider == null)
            {
                throw new KeyNotFoundException($"Provider with ID '{service.ProviderId}' not found.");
            }
            if (service.BasePrice > service.Price)
            {
                throw new InvalidOperationException("The base price cannot be higher than the final retail price.");
            }

            if (service.DurationMinutes <= 0)
            {
                throw new InvalidOperationException("Service duration must be greater than 0 minutes.");
            }

            var serviceEntity = _mapper.Map<Service>(service);
            var create = await _serviceRepository.CreateAsync(serviceEntity);
            return _mapper.Map<ServiceDTO>(create);
        }
        public async Task<ServiceDTO?> UpdateService(Guid id, ServiceUpdateRequest updatedService)
        {
            var existingService = await _serviceRepository.GetByIdAsync(id);
            if (existingService == null)
            {
                return null; 
            }

            if (updatedService.BasePrice > updatedService.Price)
            {
                throw new InvalidOperationException("The base price cannot be higher than the final retail price.");
            }
            _mapper.Map(updatedService, existingService);
            await _serviceRepository.UpdateAsync(existingService);
            return _mapper.Map<ServiceDTO>(existingService);
        }
        public async Task<bool> DeleteService(Guid id)
        {
            var existingService = await _serviceRepository.GetByIdAsync(id);
            if (existingService == null)
            {
                return false;
            }
            existingService.Status = Domain.Enums.ServiceStatus.Removed;
            await _serviceRepository.UpdateAsync(existingService);
            return true;
        }

        public async Task<IEnumerable<ServiceDTO>> GetMyProviderServices(Guid userId)
        {
            var providerUser = await _providerUserRepository.GetByUserIdAsync(userId);
            if (providerUser == null)
            {
                throw new KeyNotFoundException($"You are not associated with any provider.");
            }
            var provider = providerUser.Provider;
            if (provider == null)
            {
                throw new KeyNotFoundException($"You are not associated with any provider.");
            }
            return _mapper.Map<IEnumerable<ServiceDTO>>(await _serviceRepository.GetByProviderIdAsync(provider.Id));
        }
    }
    }
