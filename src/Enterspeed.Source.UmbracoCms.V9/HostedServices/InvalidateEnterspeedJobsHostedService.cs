﻿using System;
using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.HostedServices;

namespace Enterspeed.Source.UmbracoCms.V9.HostedServices
{
    public class InvalidateEnterspeedJobsHostedService : RecurringHostedServiceBase
    {
        private readonly IRuntimeState _runtimeState;
        private readonly IEnterspeedJobHandler _enterspeedJobHandler;
        private readonly ILogger<InvalidateEnterspeedJobsHostedService> _logger;
        private readonly IServerRoleAccessor _serverRoleAccessor;
        private readonly IEnterspeedConfigurationService _configurationService;
        private readonly IScopeProvider _scopeProvider;

        public InvalidateEnterspeedJobsHostedService(
            IRuntimeState runtimeState,
            IEnterspeedJobHandler enterspeedJobHandler,
            ILogger<InvalidateEnterspeedJobsHostedService> logger,
            IServerRoleAccessor serverRoleAccessor,
            IEnterspeedConfigurationService configurationService, 
            IScopeProvider scopeProvider)
            : base(TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(1))
        {
            _runtimeState = runtimeState;
            _enterspeedJobHandler = enterspeedJobHandler;
            _logger = logger;
            _serverRoleAccessor = serverRoleAccessor;
            _configurationService = configurationService;
            _scopeProvider = scopeProvider;
        }

        public override Task PerformExecuteAsync(object state)
        {
            // Don't do anything if the site is not running.
            if (_runtimeState.Level != RuntimeLevel.Run)
            {
                return Task.CompletedTask;
            }

            using IScope scope = _scopeProvider.CreateScope();
            if (!_configurationService.GetConfiguration().IsConfigured)
            {
                // Remember to complete the scope when done.
                scope.Complete();
                return Task.CompletedTask;
            }

            if (_serverRoleAccessor.CurrentServerRole == ServerRole.SchedulingPublisher || _serverRoleAccessor.CurrentServerRole == ServerRole.Single)
            {
                _enterspeedJobHandler.InvalidateOldProcessingJobs();
            }
            else
            {
                _logger.LogDebug("Does not run on servers with {role} role.", _serverRoleAccessor.CurrentServerRole.ToString());
            }

            // Remember to complete the scope when done.
            scope.Complete();
            return Task.CompletedTask;
        }
    }
}
