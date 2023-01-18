using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Rabobank.TechnicalTest.GCOB.Domain;
using Rabobank.TechnicalTest.GCOB.Exceptions;
using Rabobank.TechnicalTest.GCOB.Services;

namespace Rabobank.TechnicalTest.GCOB.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IValidator<Customer> _customerValidator;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, IValidator<Customer> customerValidator, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _customerValidator = customerValidator;
            _logger = logger;
        }

        /// <summary>
        /// Get a customer with their address.
        /// </summary>
        /// <param name="id">Customer identity</param>
        /// <returns>Customer details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces( MediaTypeNames.Application.Json)]
        public async Task<ActionResult> Get([FromRoute]int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerAsync(id);
                return Ok(customer);
            }
            catch (NotFoundException)
            {
                string message = $"No customer could be found with id: {id}";
                _logger.LogError(message);
                return NotFound(new { status = StatusCodes.Status404NotFound, message });
            }
            catch (Exception exception)
            {
                const string message = "Problem occurred while retrieving customers";
                _logger.LogError(exception, message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = StatusCodes.Status500InternalServerError, message });
            }
        }

        /// <summary>
        /// Submit new customer
        /// </summary>
        /// <param name="customer">customer details</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> Post([FromBody]Customer customer)
        {
            try
            {
                var validationResult = await _customerValidator.ValidateAsync(customer);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { status = StatusCodes.Status400BadRequest, message = "Invalid data supplied" });
                }

                var customerId = await _customerService.AddNewCustomer(customer);
                return new CreatedResult($"/customer/{customerId}", new { id = customerId });
            }
            catch (NotFoundException exception)
            {
                _logger.LogError(exception.Message);
                return BadRequest(new { status = StatusCodes.Status400BadRequest, message = "Invalid data supplied" });
            }
            catch (Exception exception)
            {
                const string message = "Problem occurred when add a new customers";
                _logger.LogError(exception, message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = StatusCodes.Status500InternalServerError, message });
            }
        }
    }
}
