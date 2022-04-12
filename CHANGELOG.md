# [1.158.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.157.0...v1.158.0) (2022-04-12)


### Features

* or-907 ui fixed for role changes ([0bf5752](https://github.com/informatievlaanderen/organisation-registry/commit/0bf575263e0c5df95dea29bd937b75b913055828))

# [1.157.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.156.0...v1.157.0) (2022-04-12)


### Features

* or-933 decentraalbeheerder instead of invoerder/beheerder ([2674dd3](https://github.com/informatievlaanderen/organisation-registry/commit/2674dd3b2ed7bee22759ad6bbdbc498a009ba582))

# [1.156.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.155.0...v1.156.0) (2022-04-12)


### Features

* or-936 don't overwrite ismainlocation on kbosync ([9b96e6b](https://github.com/informatievlaanderen/organisation-registry/commit/9b96e6b49b208f33de7d008dd901a1317486e1ea))
* or-936 unittest for ismainlocation not overwritten by kbosync ([542eecb](https://github.com/informatievlaanderen/organisation-registry/commit/542eecb567fe690962f4504898d060688f0ebae0))

# [1.155.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.154.0...v1.155.0) (2022-04-07)


### Features

* or-895 cleanup ([a5e23d0](https://github.com/informatievlaanderen/organisation-registry/commit/a5e23d00fa66bbb7dec61b8841c7355350cc5d3c))
* or-895 kbolocation can be set as mainlocation ([5214513](https://github.com/informatievlaanderen/organisation-registry/commit/5214513c86bdda4fc3876c0fc9d105c8f0bfa0bb))

# [1.154.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.153.0...v1.154.0) (2022-04-07)


### Bug Fixes

* or-921 remove lock to allow multiple hosted services to run ([9dcc8f0](https://github.com/informatievlaanderen/organisation-registry/commit/9dcc8f070e37a6b341a63edf06727ddb95391860))


### Features

* or-921 log start of sync using Information ([a5a7e5c](https://github.com/informatievlaanderen/organisation-registry/commit/a5a7e5c2563ec16907aaf001bf432d704f52b829))
* or-921 rename folder for HostedServices ([fbb3002](https://github.com/informatievlaanderen/organisation-registry/commit/fbb3002fce3996350ab88f052f1a0fa4a2af1d76))

# [1.153.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.152.1...v1.153.0) (2022-04-06)


### Bug Fixes

* or-921 use hosted service to sync kbo ([3d2cfeb](https://github.com/informatievlaanderen/organisation-registry/commit/3d2cfebd6d8ada19c21ee551c2a987ef6172f7c6))


### Features

* or-921 add wiremock config for RegistreerInschrijving ([04b7e16](https://github.com/informatievlaanderen/organisation-registry/commit/04b7e16ba81b2f5968d004a52a2fbb6b59220846))
* or-921 fix randomInts in ranges ([07a5420](https://github.com/informatievlaanderen/organisation-registry/commit/07a5420c6081e7011ee56cd71197156c2f71cba2))
* or-921 increase delay for scheduled commands ([887b5ee](https://github.com/informatievlaanderen/organisation-registry/commit/887b5ee09fea0fd7aa8701a2745b5648e3aaa50e))
* or-921 make hosted services configurable ([0f3f216](https://github.com/informatievlaanderen/organisation-registry/commit/0f3f2166ee5c207486fd1d7744370335c05d86e2))
* or-921 mock GeefOnderneming with wiremock ([667c3b6](https://github.com/informatievlaanderen/organisation-registry/commit/667c3b66a9db0f9d010165ea11881e8d6720d097))
* or-921 upgrade wiremock ([a3c5ffb](https://github.com/informatievlaanderen/organisation-registry/commit/a3c5ffb545d6236d0d8a9a907e431a2f86666c43))

## [1.152.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.152.0...v1.152.1) (2022-04-04)


### Bug Fixes

* make property get-only ([323b597](https://github.com/informatievlaanderen/organisation-registry/commit/323b597f26576a3cdc2fc8567559217766e90e63))
* or-908 trust server in integration tests ([fa2a40c](https://github.com/informatievlaanderen/organisation-registry/commit/fa2a40ccff3bb209c2640056860bb45b13aea756))
* or-930 fix mail notifs by not throwing randomly ([68194de](https://github.com/informatievlaanderen/organisation-registry/commit/68194de6bf97df81244726cef39f708c3fe91252))

# [1.152.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.151.1...v1.152.0) (2022-03-31)


### Features

* or-884 default sort by ovoNumber ([9a0e2d5](https://github.com/informatievlaanderen/organisation-registry/commit/9a0e2d5a5b4faf98573871208b2c0bec4a01aa53))

## [1.151.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.151.0...v1.151.1) (2022-03-29)


### Bug Fixes

* or-909 terminate personCapacities according to the right id ([0b087b6](https://github.com/informatievlaanderen/organisation-registry/commit/0b087b6762d5fe5b1bb7635d20677fd8cb5828fb))

# [1.151.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.150.0...v1.151.0) (2022-03-29)


### Bug Fixes

* or-852 make nullable for legacy events ([675b0fa](https://github.com/informatievlaanderen/organisation-registry/commit/675b0fa26efb41ec64cd417ee6872b310a6fed2a))
* or-909 make ES mass update work in tests ([3073f2c](https://github.com/informatievlaanderen/organisation-registry/commit/3073f2c7cde148c8de3d052262199b45d20f540f))
* or-909 renames & refactors ([5c78ea1](https://github.com/informatievlaanderen/organisation-registry/commit/5c78ea1fee809646533ad1c5264df6caa6eab4c7))


### Features

* or-909 delete indices after test run ([bc77d86](https://github.com/informatievlaanderen/organisation-registry/commit/bc77d86e3b44bf00ba38a1b6f5906e703f96dde5))
* or-910 fix appsettings ([fa552e4](https://github.com/informatievlaanderen/organisation-registry/commit/fa552e4b2093127c22e35b8335483105506bf85d))

# [1.150.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.149.1...v1.150.0) (2022-03-25)


### Features

* or-910 es tests on ci ([d076279](https://github.com/informatievlaanderen/organisation-registry/commit/d076279aaa225b53ae7e6363594432e0a4f0c1c6))

## [1.149.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.149.0...v1.149.1) (2022-03-24)


### Bug Fixes

* or-852 eventhandlers ([c037dcd](https://github.com/informatievlaanderen/organisation-registry/commit/c037dcd555e7e7fb142f7926d8e46aa2bde7d808))

# [1.149.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.148.0...v1.149.0) (2022-03-24)


### Features

* or-852 refactoring ([ff9b3d0](https://github.com/informatievlaanderen/organisation-registry/commit/ff9b3d070e54497123cbd71ad076fce54f1a8d96))

# [1.148.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.147.0...v1.148.0) (2022-03-24)


### Features

* or-852 refactor of config parameters ([441e6b6](https://github.com/informatievlaanderen/organisation-registry/commit/441e6b653842398ddec4674fb6c94c403a4ec3e0))
* or-852 terminate vlimperskey on termination of organisation ([b02b188](https://github.com/informatievlaanderen/organisation-registry/commit/b02b188760a06a9358b865c65fbcd337091744fd))

# [1.147.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.146.2...v1.147.0) (2022-03-24)


### Features

* or-698 only expose rendered description as description ([57fc580](https://github.com/informatievlaanderen/organisation-registry/commit/57fc580854750e1dcd0b7b2750a2b568a6de62af))

## [1.146.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.146.1...v1.146.2) (2022-03-22)


### Bug Fixes

* or_899 prevent mixing vlimpers and non-vlimpers on update ([6beb1d0](https://github.com/informatievlaanderen/organisation-registry/commit/6beb1d0290c802320cccaab3df5fe73a07326814))

## [1.146.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.146.0...v1.146.1) (2022-03-22)


### Bug Fixes

* or-751 use workRulesUrl instead of url ([a01d774](https://github.com/informatievlaanderen/organisation-registry/commit/a01d774ec7d5947ac5ac9f105a3466ff9d482743))

# [1.146.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.145.0...v1.146.0) (2022-03-18)


### Bug Fixes

* remove scheduler from production gh action ([4b98f44](https://github.com/informatievlaanderen/organisation-registry/commit/4b98f44c541a08222e079cfafe60bf01c0ec3237))


### Features

* or-751 add workrules pdf to regulations ([106ef7c](https://github.com/informatievlaanderen/organisation-registry/commit/106ef7cdbad5f0a390ea8b1c71509427eebdd635))
* or-751 proper validation ([b608995](https://github.com/informatievlaanderen/organisation-registry/commit/b60899511ec2cf529bdefd16296167ae14dd3abb))
* or-751 unittests ([11b0bed](https://github.com/informatievlaanderen/organisation-registry/commit/11b0bed3f7e0da3b61a3a953ddede1dd6a3774db))

# [1.145.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.144.0...v1.145.0) (2022-03-16)


### Features

* or-787 fixed elasticsearch tests (almost) ([80b3d23](https://github.com/informatievlaanderen/organisation-registry/commit/80b3d237148b1868c15b4076d12db0c91331871a))
* or-787 fixed final es test ([7e7a690](https://github.com/informatievlaanderen/organisation-registry/commit/7e7a690cd5aa0d5d963eef044b806573e0e731ec))
* or-787 improve EnvVarIgnoreFact ([2ee1532](https://github.com/informatievlaanderen/organisation-registry/commit/2ee15324137d663525d71d52ea1778aba906734b))
* or-787 resolve review comments ([82406e9](https://github.com/informatievlaanderen/organisation-registry/commit/82406e9834136209c7e00c8a1b46356101b3a84d))

# [1.144.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.143.0...v1.144.0) (2022-03-11)


### Features

* or-497 report 'vademecum vlaamse overheid' removed ([ead083e](https://github.com/informatievlaanderen/organisation-registry/commit/ead083e3751aae064bd82fb48259217e18196cbb))

# [1.143.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.142.0...v1.143.0) (2022-03-09)


### Bug Fixes

* or-765 don't use ImmutableArray while we're still building the roles ([f9ef143](https://github.com/informatievlaanderen/organisation-registry/commit/f9ef1430fdbe8c98334c8117b8d45f0cb0ddbf61))
* or-765 make developers setting explicitly nullable ([8d6384d](https://github.com/informatievlaanderen/organisation-registry/commit/8d6384df3fa995d1424587f6318876bbcec84cbf))


### Features

* invalidate cache on organisation tree update ([c643498](https://github.com/informatievlaanderen/organisation-registry/commit/c643498991b89719128e12278d44b0d3b0022992))
* or-765 cache user org information ([6528429](https://github.com/informatievlaanderen/organisation-registry/commit/6528429113497ba575b440832e164df7b83c232d))
* or-765 use ImmutableArray ([7205b0f](https://github.com/informatievlaanderen/organisation-registry/commit/7205b0f84972fc613387dd096b30ceb18711e06f))
* reset user's ui security cache on create daughter ([0c62af6](https://github.com/informatievlaanderen/organisation-registry/commit/0c62af63499de808c1fc6d7537f9444269a29dcc))

# [1.142.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.141.0...v1.142.0) (2022-03-09)


### Features

* or-507 add managed by field in search api ([1657aa6](https://github.com/informatievlaanderen/organisation-registry/commit/1657aa66224bda190aa67b7346ba9a2acaba1fc0))

# [1.141.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.140.3...v1.141.0) (2022-03-08)


### Features

* or-719 enable logging of x-forwarded-for ([691658a](https://github.com/informatievlaanderen/organisation-registry/commit/691658ae53f0e3b91a9292cc4f7da2215e1438ef))
* or-759 added osc and opensearch.net projects ([ea81231](https://github.com/informatievlaanderen/organisation-registry/commit/ea81231b8580d56b33ff470b2c95baa30ed1b5f0))
* or-759 changed all elasticsearch reference to opensearch references ([71f92e3](https://github.com/informatievlaanderen/organisation-registry/commit/71f92e3836d11f19b68efb489deca22e063a74b4))
* or-759 cleanup references ([093d469](https://github.com/informatievlaanderen/organisation-registry/commit/093d469057ddc7f3be4ab708d066b095a8a19d5e))

## [1.140.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.140.2...v1.140.3) (2022-03-07)


### Bug Fixes

* or-764 release from vlimpers in AR state ([1e5f90a](https://github.com/informatievlaanderen/organisation-registry/commit/1e5f90a547a21d8f0afa95cb354385a145a93011))

## [1.140.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.140.1...v1.140.2) (2022-03-03)


### Bug Fixes

* or-678 use formal frwk specific guard ([9f25a7d](https://github.com/informatievlaanderen/organisation-registry/commit/9f25a7d929a8f0b0184d2bb517504cbfa469b989))

## [1.140.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.140.0...v1.140.1) (2022-03-03)


### Bug Fixes

* or-678 add vlimpers explicitly to role guards ([d52d9d7](https://github.com/informatievlaanderen/organisation-registry/commit/d52d9d771d95a8a7d1440661636d59aa664bde7f))
* remove scheduler from deployment scripts ([7d641ba](https://github.com/informatievlaanderen/organisation-registry/commit/7d641ba9a8da0558e837137e10c298aac5171366))

# [1.140.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.139.0...v1.140.0) (2022-03-02)


### Features

* or-735 allow vlimpers to act on terminated vlimpers organisations ([4500134](https://github.com/informatievlaanderen/organisation-registry/commit/450013453ec665adc5da64dd6a37c236911a6ced))
* or-735 unify client-side authorization ([214e6d4](https://github.com/informatievlaanderen/organisation-registry/commit/214e6d41158fb32577b27f3be019f9816fb78be2))

# [1.139.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.138.0...v1.139.0) (2022-03-02)


### Bug Fixes

* or-723 allow tag list to be disabled ([790c836](https://github.com/informatievlaanderen/organisation-registry/commit/790c8361e8f5ffea343e99de34332a77445a0da8))
* or-723 remove listener correctly, remove disabled ([9e60f66](https://github.com/informatievlaanderen/organisation-registry/commit/9e60f66369c000e0b8b336fa65c6e34599d58b40))


### Features

* or-723 rename and change uri for 'limited to vlimpers' ([a15c05a](https://github.com/informatievlaanderen/organisation-registry/commit/a15c05ac2b5dc64238dfa8afb9e88c541bb7b462))
* or-723 split organisation info update into separate cmds ([7128b08](https://github.com/informatievlaanderen/organisation-registry/commit/7128b08b51c29f1b85c2952069cd2d62b0f66e10))

# [1.138.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.137.0...v1.138.0) (2022-03-02)


### Bug Fixes

* or-678 vlimpersbeheerder should not get beheerder role ([eaf7113](https://github.com/informatievlaanderen/organisation-registry/commit/eaf71137a2ef1d937c9b0b9ca29479b3a84dbb77))


### Features

* or-678 add identityserver login for orgaanbeheerder ([4cde5ca](https://github.com/informatievlaanderen/organisation-registry/commit/4cde5ca07b515325d3f9d93c8f70da1be92cbb1b))
* or-678 use more realistic ovo for vlimpers ([0a2cc32](https://github.com/informatievlaanderen/organisation-registry/commit/0a2cc32d9ee801c572fb84dcb3a64a69da8905cd))
* or-740 migration to fix nullable isActive column ([17c1519](https://github.com/informatievlaanderen/organisation-registry/commit/17c151989813bcace6cd36918e9264cf82d5bf84))

# [1.137.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.136.0...v1.137.0) (2022-02-28)


### Features

* or-739 correcte moeders worden getoond in rapporten ([13c9cfe](https://github.com/informatievlaanderen/organisation-registry/commit/13c9cfe0b7f565f1ecc82d82000f550e173743d0))
* or-739 refac: grote file opgesplitst ([c78e01e](https://github.com/informatievlaanderen/organisation-registry/commit/c78e01e834e78a7762c14ae001cd5f60b94eef68))

# [1.136.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.135.0...v1.136.0) (2022-02-24)


### Bug Fixes

* or-725 disallow add label btn when no rights ([95d0b80](https://github.com/informatievlaanderen/organisation-registry/commit/95d0b807b37bfedcaee9f6c7a8568652e595f979))


### Features

* or-726 always enable add key for beheerders ([0de5c9f](https://github.com/informatievlaanderen/organisation-registry/commit/0de5c9ffde59a98ee8301abb7b593e3840cb1c6d))
* or-726 change key policy for beheerders ([b0ce99a](https://github.com/informatievlaanderen/organisation-registry/commit/b0ce99a679dca249375a435e584a0873101ff097))

# [1.135.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.134.1...v1.135.0) (2022-02-23)


### Bug Fixes

* or-725 beheerders can still edit non-vlimpers labels ([1552971](https://github.com/informatievlaanderen/organisation-registry/commit/155297191f983d2019686021187dab27b3e30aab))


### Features

* or-725 exclude vlimpers labels for beheerders ([7ce0185](https://github.com/informatievlaanderen/organisation-registry/commit/7ce018507fbf1ebcfbe0737c6350619428adea6c))
* or-725 make GetSecurityInformation async ([e193497](https://github.com/informatievlaanderen/organisation-registry/commit/e1934972fecd9d62dfa4a103ac9f95b98eaf18f0))

## [1.134.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.134.0...v1.134.1) (2022-02-23)


### Bug Fixes

* or-740 fixed OrganisationCapacity projection ([7906acc](https://github.com/informatievlaanderen/organisation-registry/commit/7906acce2ba8fff62153c310d4612eb78d52deb2))

# [1.134.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.133.0...v1.134.0) (2022-02-22)


### Bug Fixes

* fixed a whole range of issues by replacing dayhaspassed event with a commandscheduler service ([46e3726](https://github.com/informatievlaanderen/organisation-registry/commit/46e3726f78e211b479dd0626df10b2e8cd238fde))


### Features

* or-711 cleanup of some loose ends ([78cf8af](https://github.com/informatievlaanderen/organisation-registry/commit/78cf8af7a0686ae1ba82d028894350686ea0b262))
* or-711 extra testcases + bugfixes ([daab2f3](https://github.com/informatievlaanderen/organisation-registry/commit/daab2f3f8958d3d2c5e5095ad549b59685a4e410))
* or-711 modified sqlservereventreader to ignore dayhaspassed events ([91bec5f](https://github.com/informatievlaanderen/organisation-registry/commit/91bec5f8d47098e9273c3770d413875395391bc2))
* or-711 null forgiving operators removed ([1194648](https://github.com/informatievlaanderen/organisation-registry/commit/11946486c6fe0f48519816f1005e5f1292470406))
* or-711 rename some tests ([9a127b6](https://github.com/informatievlaanderen/organisation-registry/commit/9a127b641a70e7664414ff34aa085569701b37ed))
* or-711 review comments ([8b20fcf](https://github.com/informatievlaanderen/organisation-registry/commit/8b20fcf25941d529a52a761090f11cf13cc2d635))

# [1.133.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.132.0...v1.133.0) (2022-02-17)


### Features

* or-729 use keyPolicy only list editable label types ([bd0cb7d](https://github.com/informatievlaanderen/organisation-registry/commit/bd0cb7d92e66d4cbb1296aec6828c39b4de4e9e4))

# [1.132.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.131.0...v1.132.0) (2022-02-17)


### Features

* or-728 introduce memory cache for underVlimpersMgmt ([5f1b3ce](https://github.com/informatievlaanderen/organisation-registry/commit/5f1b3ce3b54c0faa364780a411fe4aa40dcb2113))
* or-728 use labelPolicy only list editable label types ([03c78d6](https://github.com/informatievlaanderen/organisation-registry/commit/03c78d65bb52ce8b32270fb70a79c251969eb137))
* or-728 use labelPolicy to set isEditable ([22fb809](https://github.com/informatievlaanderen/organisation-registry/commit/22fb8090caecee27702beacd814e79731129f244))

# [1.131.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.130.1...v1.131.0) (2022-02-14)


### Bug Fixes

* or-722 allow vlimpersbeheerders to terminate orgs ([c630076](https://github.com/informatievlaanderen/organisation-registry/commit/c6300765c659f07394b0f2f7fe9e0953e1768857))
* or-724 use vlimpers policy on daughter creation ([dfe1b92](https://github.com/informatievlaanderen/organisation-registry/commit/dfe1b9235346ebd845dec1f0daa0807d8515366f))
* or-728 apply label policy ([3c3bd26](https://github.com/informatievlaanderen/organisation-registry/commit/3c3bd26272e14670e912ef0ab3041aea68272c29))
* or-729 apply key policy ([e1b5aed](https://github.com/informatievlaanderen/organisation-registry/commit/e1b5aed794bbe87d66bb7d3ea50cf54bea0cec3a))
* use correct method ([e25af95](https://github.com/informatievlaanderen/organisation-registry/commit/e25af95debd1502385657194cbd223440f1f3b03))
* use different policy when adding top-level org ([19a5f93](https://github.com/informatievlaanderen/organisation-registry/commit/19a5f935b462bf33f75f10b93b6c5544899a304d))


### Features

* only allow beheerders to update limited info ([3a6c36e](https://github.com/informatievlaanderen/organisation-registry/commit/3a6c36e2edd5314936880291cb20fc351a0f182d))
* use new policy for organisation info ([0e50ac7](https://github.com/informatievlaanderen/organisation-registry/commit/0e50ac76e93cfc60dc2ea0035e5e8a8407cbef19))

## [1.130.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.130.0...v1.130.1) (2022-02-10)


### Bug Fixes

* or-704 make naming consistent, fix edit btn ([879e133](https://github.com/informatievlaanderen/organisation-registry/commit/879e1337ebed21b44fe1f0cc8e90308d4143656b))
* or-717 subscribe to org changed for title ([dc9fc3b](https://github.com/informatievlaanderen/organisation-registry/commit/dc9fc3bb3aa00a43024f8710aff9d00f5839aad3))

# [1.130.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.129.0...v1.130.0) (2022-02-10)


### Bug Fixes

* make auth section nullable and handle it ([097930f](https://github.com/informatievlaanderen/organisation-registry/commit/097930fe10b36a0f1b79e29ce80a7bfedcb6cff1))


### Features

* apply formal framework policy to updates and gets ([aa0fba3](https://github.com/informatievlaanderen/organisation-registry/commit/aa0fba3c787f6481d3435e454e41c383f268c8c0))
* introduce authorization configuration & restructure ([3878084](https://github.com/informatievlaanderen/organisation-registry/commit/38780843bf95b7895372dca8505603599b84ffb6))

# [1.129.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.128.0...v1.129.0) (2022-02-09)


### Features

* or-663 fixed áll unsubscribes (prove me wrong ;-) ) ([6d58789](https://github.com/informatievlaanderen/organisation-registry/commit/6d58789a89d519c498363583021092a9c7ee74ec))

# [1.128.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.127.0...v1.128.0) (2022-02-08)


### Bug Fixes

* partial revert of too early committed code ([ed9f2cc](https://github.com/informatievlaanderen/organisation-registry/commit/ed9f2cce1f86344cad5945bc833b40db3e17d246))
* subscribe and unsubscribe ([958222f](https://github.com/informatievlaanderen/organisation-registry/commit/958222fb5351f7aef5c9a19da0de680074f6df76))


### Features

* remove unnecessary security checks ([97fd69f](https://github.com/informatievlaanderen/organisation-registry/commit/97fd69f6f667ebec8fc164ccd44223404c967a9a))
* rework authorization ([2c672ff](https://github.com/informatievlaanderen/organisation-registry/commit/2c672ffd541d521c8edf92a5658694a96bb1e7bc))
* rework authorization for parents in ui ([7deffd3](https://github.com/informatievlaanderen/organisation-registry/commit/7deffd302b8c1c1e094459e9d43c5e13e6b1584a))
* rework authorization in frontend ([773574b](https://github.com/informatievlaanderen/organisation-registry/commit/773574b443050a4b27881a269e3a92e8b8e394d9))

# [1.127.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.126.0...v1.127.0) (2022-02-07)


### Features

* or-711 implemented pokemon exceptionhandling in inProcessBus ([8f4526a](https://github.com/informatievlaanderen/organisation-registry/commit/8f4526ae3d372aadd3734b868f2e587fbd63f621))

# [1.126.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.125.0...v1.126.0) (2022-02-03)


### Features

* or-291 modification report 'organisaties per toepassingsgebied' (extended csv download) ([b6a34af](https://github.com/informatievlaanderen/organisation-registry/commit/b6a34af52fe54e9b5c3f4b0670ba87077774184c))

# [1.125.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.124.0...v1.125.0) (2022-02-01)


### Features

* counter resets, added meter ([2cab113](https://github.com/informatievlaanderen/organisation-registry/commit/2cab113c3e6eeb8da891f63baa2541685f61f6bb))

# [1.124.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.123.1...v1.124.0) (2022-02-01)


### Features

* or-710 reduce logging ([d76ae9f](https://github.com/informatievlaanderen/organisation-registry/commit/d76ae9f54e8d5b370cfc6e71b2601477448cff30))

## [1.123.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.123.0...v1.123.1) (2022-02-01)


### Bug Fixes

* chain and unsubscribe subscriptions ([44dca68](https://github.com/informatievlaanderen/organisation-registry/commit/44dca689340223a4b1afc0013d4ce23161d5f3e9))
* check vlimpers access in guard ([5efbd7d](https://github.com/informatievlaanderen/organisation-registry/commit/5efbd7df4550aff4c183d7d70e93289a87576630))

# [1.123.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.122.0...v1.123.0) (2022-01-31)


### Features

* add identity server ([9e6e963](https://github.com/informatievlaanderen/organisation-registry/commit/9e6e96311ee8cabc95e5f4a68e3955afda69d973))
* add identity server to compose file ([939a0a1](https://github.com/informatievlaanderen/organisation-registry/commit/939a0a14ed421c541b62d12c5b2d23b2443ae137))
* move oidc settings to appsettings.development.json ([fd28d46](https://github.com/informatievlaanderen/organisation-registry/commit/fd28d46da25ce10bda1882c1f5efdf79911d57b6))

# [1.122.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.121.1...v1.122.0) (2022-01-27)


### Features

* or-706 query to map organisations and niscodes ([3e9a300](https://github.com/informatievlaanderen/organisation-registry/commit/3e9a300e6bced9d05baf3b26c4680cd131f56e77))

## [1.121.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.121.0...v1.121.1) (2022-01-27)


### Bug Fixes

* or-707 es now uses only 1 connection ([64bb7e0](https://github.com/informatievlaanderen/organisation-registry/commit/64bb7e0673bb7a993fca5b45d6cbaf10b2ce27ef))

# [1.121.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.120.1...v1.121.0) (2022-01-25)


### Features

* move to opensearch ([03c392f](https://github.com/informatievlaanderen/organisation-registry/commit/03c392f18151751fc4ac02d97c89d63191c909ea))

## [1.120.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.120.0...v1.120.1) (2022-01-24)


### Bug Fixes

* or-663 unsubscribe from all subscriptions on destroy, hoping that this resolves the issue ([85ff28f](https://github.com/informatievlaanderen/organisation-registry/commit/85ff28fc0b84955f8aff0d9878ee4780b8027b5f))

# [1.120.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.119.0...v1.120.0) (2022-01-21)


### Features

* or-699 origanisatie zoeken op ovo-code ([7970a33](https://github.com/informatievlaanderen/organisation-registry/commit/7970a331d57d3711a967557b2938474d6534270e))
* refactoring of the remaining exceptions ([783c453](https://github.com/informatievlaanderen/organisation-registry/commit/783c453f25ace050d242d2aed39a003c1e0e0618))

# [1.119.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.118.0...v1.119.0) (2022-01-21)


### Features

* refactoring exceptions - body ([b394aa5](https://github.com/informatievlaanderen/organisation-registry/commit/b394aa50e138a7b71e7bf45f3f951dc93bced8a0))

# [1.118.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.117.1...v1.118.0) (2022-01-20)


### Features

* revisited refactoring of exceptions for organisations ([27a6ec6](https://github.com/informatievlaanderen/organisation-registry/commit/27a6ec6d896f5e6c1d6187f0cb8c83c3917a21a5))

## [1.117.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.117.0...v1.117.1) (2022-01-20)

# [1.117.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.116.0...v1.117.0) (2022-01-20)


### Features

* don't allow vlimpers and non-vlimpers in a parental relationship ([b92325b](https://github.com/informatievlaanderen/organisation-registry/commit/b92325bc5e12cefcb0c07cd939fbc52f67ef678b))

# [1.116.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.115.0...v1.116.0) (2022-01-20)


### Features

* 685 bij koppelen parent entiteit ook ovo-code tonen ([b0adc54](https://github.com/informatievlaanderen/organisation-registry/commit/b0adc54fde0deea500b191bc62baa167f4483964))

# [1.115.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.114.0...v1.115.0) (2022-01-20)


### Features

* 695 add version to list of events ([6943b1d](https://github.com/informatievlaanderen/organisation-registry/commit/6943b1d30df92a99f76bec19711e4d10a46f5564))

# [1.114.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.113.0...v1.114.0) (2022-01-19)


### Features

* properly unsubcribe from observable ([0976862](https://github.com/informatievlaanderen/organisation-registry/commit/0976862430954f6684466f6cfd3bace7820284af))

# [1.113.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.112.3...v1.113.0) (2022-01-19)


### Bug Fixes

* check vlimpers auth when adding org as parent ([32aab9e](https://github.com/informatievlaanderen/organisation-registry/commit/32aab9eefd464083657d38edef3789ffcfce90f9))


### Features

* filter events on aggregateid ([7da572d](https://github.com/informatievlaanderen/organisation-registry/commit/7da572d2eb917299dbcda682f7adb979262525ee))

## [1.112.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.112.2...v1.112.3) (2022-01-19)


### Bug Fixes

* only limit formal (short) name labels for non-vlimpers users ([c7f7538](https://github.com/informatievlaanderen/organisation-registry/commit/c7f7538aec602ae78230d87d0590a677faa39777))

## [1.112.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.112.1...v1.112.2) (2022-01-19)


### Bug Fixes

* **ui:** organisationRegistryBeheerders are not limited by Vlimpers ([a4a4a64](https://github.com/informatievlaanderen/organisation-registry/commit/a4a4a64ef4706f746d47b8e33a7e61429d9ae44c))

## [1.112.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.112.0...v1.112.1) (2022-01-18)


### Bug Fixes

* try different jira key ([9fdba01](https://github.com/informatievlaanderen/organisation-registry/commit/9fdba0149fbfcd15428ba84572036683d5001351))

# [1.112.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.111.2...v1.112.0) (2022-01-17)


### Bug Fixes

* parse article from string ([877b759](https://github.com/informatievlaanderen/organisation-registry/commit/877b7593d86cffa833cd5f771468c1fc2b7425c6))
* remove unused package ([bc15eab](https://github.com/informatievlaanderen/organisation-registry/commit/bc15eab23247a86d2b62f1fcbce9e3ead490aeb8))


### Features

* allow organisation to be placed under vlimpers management ([625f64c](https://github.com/informatievlaanderen/organisation-registry/commit/625f64c679cd455b060e94b4fd03dd20364f9017))
* apply OrganisationxxxUpdated events ([a90370f](https://github.com/informatievlaanderen/organisation-registry/commit/a90370fc8e11eefdce65464e0eafd10119012226))
* don't show edit btns when limited by Vlimpers ([7ff220b](https://github.com/informatievlaanderen/organisation-registry/commit/7ff220b5ce61139c5412915b577b1252681a474b))
* introduce UpdateOrganisationInfoNotLimitedByVlimpers ([ef40ffa](https://github.com/informatievlaanderen/organisation-registry/commit/ef40ffa5944b7c276abdb31e2fd6d1d17c1faf79))
* only vlimpersbeheerders can manage certain key and label types ([d426870](https://github.com/informatievlaanderen/organisation-registry/commit/d426870567883af99864cf76ebdc27affce0bfcb))
* place daughters under vlimpers management ([56c5454](https://github.com/informatievlaanderen/organisation-registry/commit/56c5454adcaabacd3ed33cc9865db0180e224072))
* show vlimpersBeheerder claim in ui ([cb623d8](https://github.com/informatievlaanderen/organisation-registry/commit/cb623d83ac962bc3b197ff78d9f3c17df6aacb8a))
* split OrganisationInfoUpdated ([61116ca](https://github.com/informatievlaanderen/organisation-registry/commit/61116ca037868663b5b63961995716afd8971b9f))
* split up VlaanderenBeNotifier ([933c49f](https://github.com/informatievlaanderen/organisation-registry/commit/933c49ff9130e49d7628e4c06dbc189e94380485))
* throw if unauthorized for vlimpers ([bc26cb8](https://github.com/informatievlaanderen/organisation-registry/commit/bc26cb8ea8caa6adc5744e26e43475fce89f9333))

## [1.111.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.111.1...v1.111.2) (2022-01-03)


### Bug Fixes

* allow orafin role to edit orafin keys ([5a20b7f](https://github.com/informatievlaanderen/organisation-registry/commit/5a20b7fc760c985c09f11ef14173216320091ffc))

## [1.111.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.111.0...v1.111.1) (2021-12-31)


### Bug Fixes

* keep td but hide pencil ([79da028](https://github.com/informatievlaanderen/organisation-registry/commit/79da0282ac11d1faeb81ed1c02034293f060b527))

# [1.111.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.110.1...v1.111.0) (2021-12-30)


### Features

* don't show edit btn when user can't use key type ([f2934f8](https://github.com/informatievlaanderen/organisation-registry/commit/f2934f8c56e5c81f0785c844a56623dbf0db6e96))

## [1.110.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.110.0...v1.110.1) (2021-12-29)


### Bug Fixes

* prevent user from updating orafin key ([4c972d6](https://github.com/informatievlaanderen/organisation-registry/commit/4c972d6874adbda431df36f8d4a2e8a4e82e21c4))
* use IUser for CanUseKeyType ([b05b306](https://github.com/informatievlaanderen/organisation-registry/commit/b05b3061f483264fea2e4231018efb2b629d08aa))

# [1.110.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.109.1...v1.110.0) (2021-12-29)


### Features

* limit usage of orafin key type to orafin ([ea65640](https://github.com/informatievlaanderen/organisation-registry/commit/ea65640ee7b6363e7239b98a1b6cd62c836c0330))
* only return allowed key types ([b462edc](https://github.com/informatievlaanderen/organisation-registry/commit/b462edc1a605595630fe77fb12c6c906a11b040a))

## [1.109.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.109.0...v1.109.1) (2021-12-06)


### Bug Fixes

* keep uri field consistent between create/update ([103d637](https://github.com/informatievlaanderen/organisation-registry/commit/103d637517ce2e7a7b9dd79ce40d17325f188797))
* remove unnecessary cols ([744e350](https://github.com/informatievlaanderen/organisation-registry/commit/744e350d0a0c2e7936e2a541a5aa3bdcce3e2561))
* translations ([4bea0a4](https://github.com/informatievlaanderen/organisation-registry/commit/4bea0a4b809ef908867d54e636b6da44a803a014))
* update name on org regulation updated ([eb6fb3c](https://github.com/informatievlaanderen/organisation-registry/commit/eb6fb3c4ad1c59ddfa65cd9330ffc2d145ec8ce3))

# [1.109.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.108.0...v1.109.0) (2021-11-26)


### Bug Fixes

* update validity on body seat updated ([e05d8f3](https://github.com/informatievlaanderen/organisation-registry/commit/e05d8f398c52f299b98436cd75601ff7d5128adc))


### Features

* only admins can manage regulations ([f5a9602](https://github.com/informatievlaanderen/organisation-registry/commit/f5a960247a25047c5e0aa9b08bd02222cdae0c7c))

# [1.108.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.107.0...v1.108.0) (2021-11-19)


### Features

* add managing regulation sub themas, use in org regulations ([f84e295](https://github.com/informatievlaanderen/organisation-registry/commit/f84e2954a520195cd594d7bafaa591815dc54344))
* add name; rename props ([7253324](https://github.com/informatievlaanderen/organisation-registry/commit/72533248e5137512ed65cd486a7bb4a0b282d09f))
* allow updating regulations ([04fef49](https://github.com/informatievlaanderen/organisation-registry/commit/04fef49ea1650a1fb0a5a285bb2062e484623114))
* ask confirmation to show md example ([e7177a2](https://github.com/informatievlaanderen/organisation-registry/commit/e7177a27635ac310d6626f2560ebaef932a93c4b))
* don't convert description to keep newlines ([6400281](https://github.com/informatievlaanderen/organisation-registry/commit/640028172c71f9cfab8ab3d9dfd3d71bc4186af6))
* handle OrganisationRegulationAdded in ES projections ([7335ad1](https://github.com/informatievlaanderen/organisation-registry/commit/7335ad14b2420104774a3c06fa485e524fb68470))
* rename 'regelgevingen' ([45fa5f0](https://github.com/informatievlaanderen/organisation-registry/commit/45fa5f07daceec7db39e974c12175e8e226b9acf))
* rename regulation type to regulation theme ([ff24a3e](https://github.com/informatievlaanderen/organisation-registry/commit/ff24a3e68b8a6643436c6929936957b4ea918d79))
* render markdown preview for description ([c558c18](https://github.com/informatievlaanderen/organisation-registry/commit/c558c18ad55880f8e61701594fadd09efda5c409))
* save rendered html to db ([86c8798](https://github.com/informatievlaanderen/organisation-registry/commit/86c879899213de18822d2147c68d6950cc75ca87))

# [1.107.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.106.0...v1.107.0) (2021-11-03)


### Features

* support providing key type ([524b27b](https://github.com/informatievlaanderen/organisation-registry/commit/524b27bb08e5ea4193b9bec3796b9cd6648a84d0))


### Reverts

* Revert "feat: no longer use base64 for certs" ([d836728](https://github.com/informatievlaanderen/organisation-registry/commit/d8367281b2af8c862c275f7ed300712cb9972b32))

# [1.106.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.105.0...v1.106.0) (2021-11-02)


### Features

* no longer use base64 for certs ([a1aef23](https://github.com/informatievlaanderen/organisation-registry/commit/a1aef23a3b475213a7cbb0461a7b97535afd3f4a))

# [1.105.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.104.0...v1.105.0) (2021-10-25)


### Features

* use elastic bus for individual rebuild runner ([defd7c5](https://github.com/informatievlaanderen/organisation-registry/commit/defd7c544a3b856f3379ffe558950a0a90b9c8fb))

# [1.104.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.103.5...v1.104.0) (2021-10-20)


### Features

* allow jwt timeout to be increased through config ([728d642](https://github.com/informatievlaanderen/organisation-registry/commit/728d642f34f289cf260ae5483905038b06b33241))

## [1.103.5](https://github.com/informatievlaanderen/organisation-registry/compare/v1.103.4...v1.103.5) (2021-10-15)


### Bug Fixes

* remove remaining datacontract attrs ([c429f25](https://github.com/informatievlaanderen/organisation-registry/commit/c429f25ef6793991f120dbb6bba37634fa73764c))

## [1.103.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.103.3...v1.103.4) (2021-10-15)


### Bug Fixes

* use correct date ([2d68bc6](https://github.com/informatievlaanderen/organisation-registry/commit/2d68bc6ae451593a5ee8f621a1f22c9022393edb))

## [1.103.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.103.2...v1.103.3) (2021-10-14)


### Bug Fixes

* make sure to detect prd ([4ad48d2](https://github.com/informatievlaanderen/organisation-registry/commit/4ad48d21d6ad69b3bbd90b36905d83e8e3760e29))

## [1.103.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.103.1...v1.103.2) (2021-10-14)


### Bug Fixes

* listen to relevant events for body organisation ([80a26d3](https://github.com/informatievlaanderen/organisation-registry/commit/80a26d3f43f4a46e15abd34620fea3b18d4782cf))
* use correct schema when rebuilding ([2e797bf](https://github.com/informatievlaanderen/organisation-registry/commit/2e797bf32bfe179c01982866c43732f966bff8aa))


### Reverts

* Revert "fix: clear and assign body organisations" ([f176074](https://github.com/informatievlaanderen/organisation-registry/commit/f1760747eaa7512942d9ad190373fc05a1473da1))

## [1.103.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.103.0...v1.103.1) (2021-10-12)


### Bug Fixes

* clear and assign body organisations ([bd74c2b](https://github.com/informatievlaanderen/organisation-registry/commit/bd74c2b37f50bbea994b3dffbb8b0cfc3abb7940))

# [1.103.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.102.0...v1.103.0) (2021-10-11)


### Features

* add user info to body notification mails ([7e10917](https://github.com/informatievlaanderen/organisation-registry/commit/7e10917025de901287b821ec4ee7718a67b89e34))

# [1.102.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.101.0...v1.102.0) (2021-10-10)


### Bug Fixes

* remove datacontract ([a65df7f](https://github.com/informatievlaanderen/organisation-registry/commit/a65df7f9993714455bffe0b9b5ccc04ab3eeea5c))


### Features

* add first draft of search docs ([6a293fc](https://github.com/informatievlaanderen/organisation-registry/commit/6a293fc9546c1957c5436006f8aa2d59bb245c8b))

# [1.101.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.100.0...v1.101.0) (2021-10-07)


### Bug Fixes

* remove datacontracts attribute ([fd0f1fe](https://github.com/informatievlaanderen/organisation-registry/commit/fd0f1fe32d91d5290d8e09e65781d1f3cf263612))


### Features

* improve swagger docs ([f629997](https://github.com/informatievlaanderen/organisation-registry/commit/f629997e220f4006df04fa5c166542b3da284068))
* make edit api require authorization ([8726659](https://github.com/informatievlaanderen/organisation-registry/commit/872665915bc672fc40cacd05c3d4b0a2966f44a3))
* upgrade build pipeline ([19add00](https://github.com/informatievlaanderen/organisation-registry/commit/19add0005a229deefc7f7581f7cb3ff22dfdbc26))

# [1.100.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.99.0...v1.100.0) (2021-10-05)


### Features

* make swagger docs work again ([78028a3](https://github.com/informatievlaanderen/organisation-registry/commit/78028a391acf26bbb69dbd655fa9cda90b496c69))

# [1.99.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.98.0...v1.99.0) (2021-10-04)


### Features

* allow machine-to-machine auth ([e21c859](https://github.com/informatievlaanderen/organisation-registry/commit/e21c859d97543f5893ea9c36fbfa7e64909f82de))

# [1.98.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.97.2...v1.98.0) (2021-10-04)


### Features

* check if user is allowed to edit key type ([60ddb70](https://github.com/informatievlaanderen/organisation-registry/commit/60ddb7024c09e89da71532df79da368a1e0e9cdf))
* introduce edit api for organisation keys ([4f6d234](https://github.com/informatievlaanderen/organisation-registry/commit/4f6d234fea6da6b579668533d21aeedb54c5762f))

## [1.97.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.97.1...v1.97.2) (2021-09-27)


### Bug Fixes

* temp disable pushing rebuilder to ecr ([015dd69](https://github.com/informatievlaanderen/organisation-registry/commit/015dd69a717202a785c77dacad80ffd0d9e56afc))

## [1.97.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.97.0...v1.97.1) (2021-09-27)


### Bug Fixes

* use bulk index for large nr of docs ([58fa750](https://github.com/informatievlaanderen/organisation-registry/commit/58fa7501cbb948c7650dd1ec8eb83a0789eaba50))

# [1.97.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.96.2...v1.97.0) (2021-09-23)


### Features

* push rebuilder to staging ecr ([383b03f](https://github.com/informatievlaanderen/organisation-registry/commit/383b03f54de8989f6f9af18ab903d5dbc831c131))

## [1.96.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.96.1...v1.96.2) (2021-09-23)


### Bug Fixes

* pass connection string ([9ca6202](https://github.com/informatievlaanderen/organisation-registry/commit/9ca620272689ad6acd11161a7f1a93c3e9600b9f))

## [1.96.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.96.0...v1.96.1) (2021-09-23)


### Bug Fixes

* ensure schema ([65e2610](https://github.com/informatievlaanderen/organisation-registry/commit/65e26104b2c90d2b968a0c815127b08efbea61ca))

# [1.96.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.95.3...v1.96.0) (2021-09-23)


### Bug Fixes

* ensure migrations history moved before migrating ([0c5b12c](https://github.com/informatievlaanderen/organisation-registry/commit/0c5b12c9f47504dee402c20f0d05d462d2551885))
* invocation of verify ([b977951](https://github.com/informatievlaanderen/organisation-registry/commit/b977951110a9f9235784f69196062f0e76032c47))


### Features

* add rebuilder project ([bcb9891](https://github.com/informatievlaanderen/organisation-registry/commit/bcb9891df9bc82f41300227f9db00889a4082d59))


### Reverts

* Revert "fix: revert moving to separate schemas for now" ([713a26e](https://github.com/informatievlaanderen/organisation-registry/commit/713a26ee4d5f28bf4d91e9ea2a7586bc2f5ad9c4))

## [1.95.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.95.2...v1.95.3) (2021-09-22)


### Bug Fixes

* choose schema for migrations ([a34365c](https://github.com/informatievlaanderen/organisation-registry/commit/a34365c287b375535dc12af7b5e83e989f91b707))
* revert moving to separate schemas for now ([53d233d](https://github.com/informatievlaanderen/organisation-registry/commit/53d233d1e94a5ab3105ae3c0eac76c94980f5070))

## [1.95.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.95.1...v1.95.2) (2021-09-22)


### Bug Fixes

* only output specific output ([1051780](https://github.com/informatievlaanderen/organisation-registry/commit/1051780856351fc57e1a4ffa2cb3c4b21614abca))

## [1.95.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.95.0...v1.95.1) (2021-09-22)


### Bug Fixes

* test with static string ([94940da](https://github.com/informatievlaanderen/organisation-registry/commit/94940dab0d77e20f4f2265359c85806697053580))

# [1.95.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.13...v1.95.0) (2021-09-22)


### Bug Fixes

* explicitly pass optional arguments in expression ([11805d7](https://github.com/informatievlaanderen/organisation-registry/commit/11805d7916b0c2f995430093a2a382d347ff9523))
* invocation of verify ([f69faf1](https://github.com/informatievlaanderen/organisation-registry/commit/f69faf105c65511b603c72b0e34fc0e797f2a2da))
* try toJson needs ([132e54a](https://github.com/informatievlaanderen/organisation-registry/commit/132e54a9e5c431c5d79d2d934f3c179cf4f4b1be))
* use correct date to compare with ([1ef9c85](https://github.com/informatievlaanderen/organisation-registry/commit/1ef9c850552e928986687cf66cee9a9a7096acbb))


### Features

* move tables into more specific schemas ([f06757f](https://github.com/informatievlaanderen/organisation-registry/commit/f06757f2685d86776af2f9cefbe5f7e101a19646))

## [1.94.13](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.12...v1.94.13) (2021-09-22)


### Bug Fixes

* debug passing outputs ([b90d8a4](https://github.com/informatievlaanderen/organisation-registry/commit/b90d8a4af1ec19839ac6623f8552d0dbcb1e553a))

## [1.94.12](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.11...v1.94.12) (2021-09-22)


### Bug Fixes

* try passing info between jobs ([bb78411](https://github.com/informatievlaanderen/organisation-registry/commit/bb78411400a5bfdd6a08aa631ea34087c75be777))

## [1.94.11](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.10...v1.94.11) (2021-09-21)


### Bug Fixes

* bad substitution ([cbbb69b](https://github.com/informatievlaanderen/organisation-registry/commit/cbbb69bb391afc5c5de990d37353c94acbfd9e0f))
* bad substitution ([168b99a](https://github.com/informatievlaanderen/organisation-registry/commit/168b99a88f25259acac77401ef6e4df7c379993e))
* repo name ([fa48c25](https://github.com/informatievlaanderen/organisation-registry/commit/fa48c252534ff89557a3079baeed1f32f55ddef7))
* try latest-release ([b9445c1](https://github.com/informatievlaanderen/organisation-registry/commit/b9445c1434b128ba35edc1c3a3cb28988993f121))

## [1.94.10](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.9...v1.94.10) (2021-09-21)


### Bug Fixes

* try dependent workflow & debug workflow_run ([e268bf5](https://github.com/informatievlaanderen/organisation-registry/commit/e268bf50073612ec0940f9da8015bf3f6c00ef19))

## [1.94.9](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.8...v1.94.9) (2021-09-21)


### Bug Fixes

* try workflow_run ([e0925ec](https://github.com/informatievlaanderen/organisation-registry/commit/e0925ec9e097c56aeca596ee8dc57c4219ac0bb6))

## [1.94.8](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.7...v1.94.8) (2021-09-20)


### Bug Fixes

* use correct name ([243c147](https://github.com/informatievlaanderen/organisation-registry/commit/243c147611f325218b787419664d7fa47d802847))

## [1.94.7](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.6...v1.94.7) (2021-09-20)


### Bug Fixes

* set output ([05501af](https://github.com/informatievlaanderen/organisation-registry/commit/05501afe4a5a55caade4336378736cd5340874e9))

## [1.94.6](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.5...v1.94.6) (2021-09-20)


### Bug Fixes

* use bash ([516755e](https://github.com/informatievlaanderen/organisation-registry/commit/516755e64110b4af28e46847d751008826a44402))

## [1.94.5](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.4...v1.94.5) (2021-09-20)


### Bug Fixes

* pass env vars explicitly ([1f65519](https://github.com/informatievlaanderen/organisation-registry/commit/1f65519b927aaac26cad4130b4716dd70de31a80))

## [1.94.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.3...v1.94.4) (2021-09-20)


### Bug Fixes

* use different syntax for GITHUB_REF ([43339a3](https://github.com/informatievlaanderen/organisation-registry/commit/43339a39a06f581c71d20827908ca8e1b8726970))

## [1.94.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.2...v1.94.3) (2021-09-17)


### Bug Fixes

* use gezondheidenzorg ([04e44aa](https://github.com/informatievlaanderen/organisation-registry/commit/04e44aa7983456617f8ab94b7dfc8c004d212801))

## [1.94.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.1...v1.94.2) (2021-09-08)


### Bug Fixes

* use personal access token ([19e3dfc](https://github.com/informatievlaanderen/organisation-registry/commit/19e3dfcd1642453f83d48529cc05e491f96047f2))

## [1.94.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.94.0...v1.94.1) (2021-09-08)

# [1.94.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.93.0...v1.94.0) (2021-09-08)


### Bug Fixes

* use correct name for gezondheidenzorg ([f8cfca2](https://github.com/informatievlaanderen/organisation-registry/commit/f8cfca2e18df791c85bf8fc5e8c0b98e09880f04))


### Features

* add github action to deploy to staging ([01fe5bc](https://github.com/informatievlaanderen/organisation-registry/commit/01fe5bc66b28fb3729b6c0aae105fe4778d26209))

# [1.93.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.92.0...v1.93.0) (2021-09-07)


### Features

* show user a warning if on dev/staging ([ee70bed](https://github.com/informatievlaanderen/organisation-registry/commit/ee70bedf6fb4aca5638439deea6f61e1971618cb))

# [1.92.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.91.1...v1.92.0) (2021-09-07)


### Features

* parents are no longer terminated ([c9da676](https://github.com/informatievlaanderen/organisation-registry/commit/c9da676cddca92bd6f2afce776abf3d6861bcbda))

## [1.91.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.91.0...v1.91.1) (2021-08-30)


### Bug Fixes

* make operational validity optional in es doc ([524aa92](https://github.com/informatievlaanderen/organisation-registry/commit/524aa922f4bc7a98a575c76b34f8ba62d148ec0b))

# [1.91.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.90.0...v1.91.0) (2021-08-17)


### Features

* send mail when balanced participation changes ([8f52b2d](https://github.com/informatievlaanderen/organisation-registry/commit/8f52b2d49dfe44382ce06607ba69b487f513ff24))

# [1.90.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.89.0...v1.90.0) (2021-08-17)


### Features

* end operational validity on organisation termination ([061eea8](https://github.com/informatievlaanderen/organisation-registry/commit/061eea82bd1c4768f38f24ade0c8196a084a6c10))

# [1.89.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.88.0...v1.89.0) (2021-08-17)


### Bug Fixes

* make sure organisations are distinct ([eded4c9](https://github.com/informatievlaanderen/organisation-registry/commit/eded4c94af810851c5d261f0f755e7193240117a))


### Features

* add article to organisation ([135f36c](https://github.com/informatievlaanderen/organisation-registry/commit/135f36c12f9e0203cc8d7db1756698a5b0196125))
* add formal validity and operational validity ([e658911](https://github.com/informatievlaanderen/organisation-registry/commit/e6589115bc688ec9f451815c46d9d201ac93d05a))
* rename validity to formal validity (ui only) ([38b6c1c](https://github.com/informatievlaanderen/organisation-registry/commit/38b6c1c4723881416683bb64d73593825017590a))

# [1.88.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.87.3...v1.88.0) (2021-08-11)


### Bug Fixes

* allow AutomatedTasks to change orgs even when terminated ([960c31b](https://github.com/informatievlaanderen/organisation-registry/commit/960c31ba600714a40f8755bad8ca1471d9a3664e))
* pass user i/o relying on ambient context ([733e5fe](https://github.com/informatievlaanderen/organisation-registry/commit/733e5fe96bd493cb4a27f614cb19d5e68122eea1))


### Features

* allow passing user explicitly to store ([5100e3f](https://github.com/informatievlaanderen/organisation-registry/commit/5100e3fb0eca3a7de23cd5dae3dd743f11aa3a98))
* require User to be passed to Session on commit ([a1b87e0](https://github.com/informatievlaanderen/organisation-registry/commit/a1b87e0c8778dec08efc9cf8f4d42e2be987d4f1))
* track IsActive in OrganisationCapacityListItemView ([df09b0f](https://github.com/informatievlaanderen/organisation-registry/commit/df09b0fae97a1f515ebb406fda2db57a38ee6d0a))

## [1.87.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.87.2...v1.87.3) (2021-07-26)


### Bug Fixes

* rename feature toggle ([1f90fd8](https://github.com/informatievlaanderen/organisation-registry/commit/1f90fd8daec6d2b082649f2d66d9d0fb436c66d7))
* typo ([705d508](https://github.com/informatievlaanderen/organisation-registry/commit/705d5085c290b2031bf53e7746eedd47f7b684bd))

## [1.87.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.87.1...v1.87.2) (2021-07-23)


### Bug Fixes

* use more findasync io single to lookup in local ([6b86e29](https://github.com/informatievlaanderen/organisation-registry/commit/6b86e29bd534b4d9f6854f259705c35dc4a15e66))

## [1.87.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.87.0...v1.87.1) (2021-07-23)


### Bug Fixes

* use findasync io single to lookup in local ([80037c3](https://github.com/informatievlaanderen/organisation-registry/commit/80037c359aa4cad8a3d762ea3009fdc5ea794134))

# [1.87.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.86.1...v1.87.0) (2021-07-22)


### Bug Fixes

* typo ([4f3fdf5](https://github.com/informatievlaanderen/organisation-registry/commit/4f3fdf546a76892ce59399581295992788db4670))


### Features

* add feature flag for regulations ([53d8809](https://github.com/informatievlaanderen/organisation-registry/commit/53d88093149089cd5258d0f906ee3e4a0ff0294a))

## [1.86.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.86.0...v1.86.1) (2021-07-22)


### Bug Fixes

* use findasync io single to lookup in local ([f7154bb](https://github.com/informatievlaanderen/organisation-registry/commit/f7154bb07ad3d2e9f63e79b5c1780494483f44e7))

# [1.86.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.85.2...v1.86.0) (2021-07-02)


### Features

* add regulation and regulation types ([8e686a6](https://github.com/informatievlaanderen/organisation-registry/commit/8e686a67e601c7ae5be8a06bf041f17729b00a92))

## [1.85.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.85.1...v1.85.2) (2021-06-08)


### Bug Fixes

* check existing organisationperbody ([3d9f0d1](https://github.com/informatievlaanderen/organisation-registry/commit/3d9f0d10d8e9a5c1919646f68e783f453e8969c5))

## [1.85.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.85.0...v1.85.1) (2021-06-08)


### Bug Fixes

* don't log incorrect number of batches ([d397134](https://github.com/informatievlaanderen/organisation-registry/commit/d397134625b5a1aba59862d07bb0beb9f493d063))
* handle BodySeatUpdated ([ba7a2be](https://github.com/informatievlaanderen/organisation-registry/commit/ba7a2be0f1d7b23f94fd5c3b06afa5f4b904e293))
* update cache on BodySeatUpdated ([e281700](https://github.com/informatievlaanderen/organisation-registry/commit/e281700906734161130d712c4ed20a7f96e8c99c))

# [1.85.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.84.1...v1.85.0) (2021-06-04)


### Features

* don't log updates to the projection state ([f2dc4b5](https://github.com/informatievlaanderen/organisation-registry/commit/f2dc4b59b4f3b981e6a272ec913fa10543862e4d))
* separate document creation from changes ([412eef9](https://github.com/informatievlaanderen/organisation-registry/commit/412eef9c1f91ff0a56cb62b6d8193a822d78566a))

## [1.84.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.84.0...v1.84.1) (2021-06-03)


### Bug Fixes

* use person id i/o body id for person doc ([4dd32e3](https://github.com/informatievlaanderen/organisation-registry/commit/4dd32e3918be050ccb8d771c298abf9e76e4ca93))

# [1.84.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.83.0...v1.84.0) (2021-06-03)


### Bug Fixes

* move to 5.0.6 ([1746140](https://github.com/informatievlaanderen/organisation-registry/commit/17461402b2a663338d41a5641f60f52e8729a808))
* register missing problem details helper ([540ee30](https://github.com/informatievlaanderen/organisation-registry/commit/540ee30a10045ac4e308213e05922263f864fe95))
* update seat cache for body mandates ([cbef748](https://github.com/informatievlaanderen/organisation-registry/commit/cbef748dcfe6be097924bd6253fd0a0226a80872))


### Features

* add body seat type name to body report ([e68f48e](https://github.com/informatievlaanderen/organisation-registry/commit/e68f48e03180cbe5859b00d8e57bf5351ae0944d))
* extract scroll to separate method ([ddd9efd](https://github.com/informatievlaanderen/organisation-registry/commit/ddd9efd968cf1d6baed5bc79c01f00b13bdb4cfe))

# [1.83.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.82.0...v1.83.0) (2021-05-27)


### Features

* add seat type name to delegations ([530564b](https://github.com/informatievlaanderen/organisation-registry/commit/530564b232a94bdc33a165b5135048356a341174))

# [1.82.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.81.4...v1.82.0) (2021-05-27)


### Features

* log event number with exception ([16074f2](https://github.com/informatievlaanderen/organisation-registry/commit/16074f298642196c2b9ffc64d4bb0e0290c147fb))

## [1.81.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.81.3...v1.81.4) (2021-05-27)


### Bug Fixes

* only update projection state after applying mass change ([6c8b3d6](https://github.com/informatievlaanderen/organisation-registry/commit/6c8b3d64567f373442bc384e2bd5e6d49004642f))

## [1.81.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.81.2...v1.81.3) (2021-05-27)


### Bug Fixes

* throw from inside async closure ([6c06643](https://github.com/informatievlaanderen/organisation-registry/commit/6c06643afd6ba805c387a6d29cae91b1a1a279ac))

## [1.81.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.81.1...v1.81.2) (2021-05-27)


### Bug Fixes

* use correct handler for ProjectionFullName ([7ebba2f](https://github.com/informatievlaanderen/organisation-registry/commit/7ebba2fddf96ee1806aeec9ce9af39a66e94a14f))

## [1.81.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.81.0...v1.81.1) (2021-05-26)


### Bug Fixes

* update projection state in batch ([24c4ba6](https://github.com/informatievlaanderen/organisation-registry/commit/24c4ba63acf2e78cf5d8efc13340e6cc09b65ab3))

# [1.81.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.80.0...v1.81.0) (2021-05-26)


### Features

* add kbo raw controller for debugging purposes ([8bc3c81](https://github.com/informatievlaanderen/organisation-registry/commit/8bc3c8102e7b9436cb55611ea3da086d027e03a7))
* update projection state immediately after flushing to ES ([77e57c4](https://github.com/informatievlaanderen/organisation-registry/commit/77e57c4e4a565c846932418229e4447c2d314d69))

# [1.80.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.79.1...v1.80.0) (2021-05-26)


### Features

* unify runners to use slow approach to rebuilding ([0287f23](https://github.com/informatievlaanderen/organisation-registry/commit/0287f230e439c7383c02ea8a0c373282a6fd0a0a))

## [1.79.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.79.0...v1.79.1) (2021-05-20)


### Bug Fixes

* use db for config ([789aa62](https://github.com/informatievlaanderen/organisation-registry/commit/789aa62ae75a547b4030925f824e4ab95c8e732b))

# [1.79.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.78.0...v1.79.0) (2021-05-20)


### Bug Fixes

* don't prepare index in ctor ([86c273e](https://github.com/informatievlaanderen/organisation-registry/commit/86c273ee42b2eb97a0c29aebe376757d9ad1d3d8))


### Features

* use async overloads wherever possible ([f3a1620](https://github.com/informatievlaanderen/organisation-registry/commit/f3a1620f3cbf1d3992ba609ec1e8fd1361252a9b))
* use batching for es people and orgs handlers ([632a357](https://github.com/informatievlaanderen/organisation-registry/commit/632a357671d35d55d4025cfa127fcd1fdf1f7375))

# [1.78.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.77.0...v1.78.0) (2021-05-10)


### Bug Fixes

* add default metrics to test ([0ccd434](https://github.com/informatievlaanderen/organisation-registry/commit/0ccd434ddde42d662603c6779f4d273161b9ea0f))
* remove unnecessary index refreshes ([96274ef](https://github.com/informatievlaanderen/organisation-registry/commit/96274ef201a8202d2d04d74e8cd418e8c622890f))


### Features

* introduce simple metrics ([967b3cd](https://github.com/informatievlaanderen/organisation-registry/commit/967b3cd3d5f66de9fb85846c70923763f6ac552b))

# [1.77.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.76.0...v1.77.0) (2021-05-10)


### Features

* introduce batching for ES projections ([c2ac169](https://github.com/informatievlaanderen/organisation-registry/commit/c2ac16979931d5cd9a47d9f337aefd8c77198dd6))
* update es version in local docker env ([e7f0054](https://github.com/informatievlaanderen/organisation-registry/commit/e7f0054f9307383620546147c106300579e7421b))

# [1.76.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.75.0...v1.76.0) (2021-05-05)


### Features

* temporarily refresh indices ([2084fbf](https://github.com/informatievlaanderen/organisation-registry/commit/2084fbfeb84bd04912756f29a213b1f9496d296e))

# [1.75.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.74.0...v1.75.0) (2021-05-03)


### Bug Fixes

* use correct cache ([30b155f](https://github.com/informatievlaanderen/organisation-registry/commit/30b155f6293c354896b9b280f9609c8c1e7204da))


### Features

* don't use memory caches for ES ([669c464](https://github.com/informatievlaanderen/organisation-registry/commit/669c464eeed3097941adb5f27f4651ddfcbc1747))

# [1.74.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.73.2...v1.74.0) (2021-04-28)


### Features

* add seat type name to body mandate view ([bd902e8](https://github.com/informatievlaanderen/organisation-registry/commit/bd902e80dba86c8ed3053db243e58393f34df49e))

## [1.73.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.73.1...v1.73.2) (2021-04-26)


### Bug Fixes

* use correct capitalization ([13d76dd](https://github.com/informatievlaanderen/organisation-registry/commit/13d76dd8f4dcbbcbf9dc2cdb9a85482c2a2586c4))

## [1.73.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.73.0...v1.73.1) (2021-04-26)


### Bug Fixes

* allow sorting participation summary ([a6c09a8](https://github.com/informatievlaanderen/organisation-registry/commit/a6c09a8df1594deaa191abacde448c8bdf1e1976))

# [1.73.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.72.0...v1.73.0) (2021-04-21)


### Features

* add sensible defaults for staging/prd logging ([895413d](https://github.com/informatievlaanderen/organisation-registry/commit/895413d810c30d0219da74737379d06cb411717b))

# [1.72.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.71.3...v1.72.0) (2021-04-21)


### Features

* reduce async network io for memory caches ([c00b1f3](https://github.com/informatievlaanderen/organisation-registry/commit/c00b1f3eae7d378e2d2f87a9c32c835bf5c160a7))

## [1.71.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.71.2...v1.71.3) (2021-04-20)


### Bug Fixes

* update existing item in cache io trying to add again ([298b9bb](https://github.com/informatievlaanderen/organisation-registry/commit/298b9bbdfd792f288b16f3e605741bacc84ff679))

## [1.71.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.71.1...v1.71.2) (2021-04-20)


### Bug Fixes

* copy paste error ([00c76b3](https://github.com/informatievlaanderen/organisation-registry/commit/00c76b39585129c4b502411fce21f58bfd4267de))

## [1.71.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.71.0...v1.71.1) (2021-04-20)


### Bug Fixes

* copy paste error ([130b2c8](https://github.com/informatievlaanderen/organisation-registry/commit/130b2c8517a2bbf34b370f71e25d239bb2eb113d))

# [1.71.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.70.1...v1.71.0) (2021-04-19)


### Bug Fixes

* add missing param ([897f8ff](https://github.com/informatievlaanderen/organisation-registry/commit/897f8ffb02b70e3a9e55c9e585128d606f518585))


### Features

* set user on command ([0e0d222](https://github.com/informatievlaanderen/organisation-registry/commit/0e0d222e86543d0faa702d764ce1a988652954bb))

## [1.70.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.70.0...v1.70.1) (2021-04-16)


### Bug Fixes

* only return first location ([4935eaa](https://github.com/informatievlaanderen/organisation-registry/commit/4935eaaaedf35a774e8c5fa2f1df5225d3377fa4))

# [1.70.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.69.0...v1.70.0) (2021-04-15)


### Bug Fixes

* don't re-throw so other runners can still run ([6b11e6a](https://github.com/informatievlaanderen/organisation-registry/commit/6b11e6aff45187e55f50c1ceafca64740119b21a))


### Features

* run individual runner first and until current projection number ([a6a9d16](https://github.com/informatievlaanderen/organisation-registry/commit/a6a9d162846aa43a9e4b673dbfc81752bd200cfe))

# [1.69.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.68.1...v1.69.0) (2021-04-14)


### Features

* add FromVersion ([89ef91c](https://github.com/informatievlaanderen/organisation-registry/commit/89ef91c226358d0ec4c27a1f54a9514ff65b5e19))
* add individual rebuild runner for ES ([5c497f9](https://github.com/informatievlaanderen/organisation-registry/commit/5c497f9c827c44da60d36db3216de1ad088065d7))
* add method to get all events for an id ([bd8a6da](https://github.com/informatievlaanderen/organisation-registry/commit/bd8a6da612b2b6988ce5e7240556d296f996fff2))
* add OrganisationsToRebuild and move to ES schema ([61051bc](https://github.com/informatievlaanderen/organisation-registry/commit/61051bcc20d5c9b41cfabf974a297506daa46779))
* add production settings for serilog ([4c00b9e](https://github.com/informatievlaanderen/organisation-registry/commit/4c00b9e912f3c7939de3f3b5af654bd23a49c22e))

## [1.68.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.68.0...v1.68.1) (2021-04-13)


### Reverts

* Revert "feat: print env variables for serilog debug" ([efd1fd4](https://github.com/informatievlaanderen/organisation-registry/commit/efd1fd4b45371f2fbbfd6399f3cf1c2d526f4579))

# [1.68.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.67.1...v1.68.0) (2021-04-13)


### Features

* print env variables for serilog debug ([105644d](https://github.com/informatievlaanderen/organisation-registry/commit/105644d2ed9b56e609cf0ffd61140819321e509c))

## [1.67.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.67.0...v1.67.1) (2021-04-13)


### Bug Fixes

* correctly handle async exceptions ([5394957](https://github.com/informatievlaanderen/organisation-registry/commit/539495710679a13213faba7a0d3069e1aa37779a))
* override ToString ([193374a](https://github.com/informatievlaanderen/organisation-registry/commit/193374a1be87c25889de7fa17010293eaf95e5dd))

# [1.67.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.66.0...v1.67.0) (2021-04-09)


### Bug Fixes

* use kbo property instead of outdated key ([9586389](https://github.com/informatievlaanderen/organisation-registry/commit/9586389d0530a2d48e7cae70065eecfddffcae35))


### Features

* expand readme with bcp commands ([fe9e706](https://github.com/informatievlaanderen/organisation-registry/commit/fe9e706ee107d5c48d0c068f8c241055ad711a4f))

# [1.66.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.65.0...v1.66.0) (2021-04-08)


### Bug Fixes

* use currently valid properties ([3f5cb93](https://github.com/informatievlaanderen/organisation-registry/commit/3f5cb93a13e7064d6f2f3f10150bfbdea432ba4d))


### Features

* expand readme ([92a6f94](https://github.com/informatievlaanderen/organisation-registry/commit/92a6f9432bb68ee4de14269338177ae8b4ec2a20))

# [1.65.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.64.0...v1.65.0) (2021-04-06)


### Features

* add security service for use in projections ([b382228](https://github.com/informatievlaanderen/organisation-registry/commit/b38222861512023026070c0a609c0748ab596e98))

# [1.64.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.63.1...v1.64.0) (2021-03-09)


### Bug Fixes

* apply correct events ([074995e](https://github.com/informatievlaanderen/organisation-registry/commit/074995e8141fae804311495e709281b43aecd337))
* don't clear kbo fields if not forced ([365561f](https://github.com/informatievlaanderen/organisation-registry/commit/365561fb71837b8ffefe93723bd9cf313cff6247))
* only fetch items to terminate ([fde93b6](https://github.com/informatievlaanderen/organisation-registry/commit/fde93b64c08c4bb801ae672de8cc9b9758e944cf))
* only update updated contacts ([42aac40](https://github.com/informatievlaanderen/organisation-registry/commit/42aac407f239de81d5a908e047192b75732eb6d1))
* typo ([f0901ec](https://github.com/informatievlaanderen/organisation-registry/commit/f0901ec39885459300f0dcdb75f336507e09d822))
* unsubscribe OnDestroy ([ae35b88](https://github.com/informatievlaanderen/organisation-registry/commit/ae35b882d3e801b9e7f28885ce4624f93445a58e))
* use correct Id for organisation parents ([5d17cd4](https://github.com/informatievlaanderen/organisation-registry/commit/5d17cd4a432aa9991cd4819a415d7d41791ad27f))
* use store for isEditable ([6fbae05](https://github.com/informatievlaanderen/organisation-registry/commit/6fbae0599506d10e620c6811a0d14d1e6d6298c9))


### Features

* add IsTerminated to OrgDetailItemView ([477bc0d](https://github.com/informatievlaanderen/organisation-registry/commit/477bc0d333d6175d17083d95dffe35288c6daac9))
* add kbo specific terminations, handle in AR ([cfe261c](https://github.com/informatievlaanderen/organisation-registry/commit/cfe261c0888ee63f609aa156c163cedfbe9b6736))
* add role restriction and date to terminate ctrl method ([da804a8](https://github.com/informatievlaanderen/organisation-registry/commit/da804a8423176b703bcec835d393886505cf0787))
* add simple ui to terminate organisation ([706610a](https://github.com/informatievlaanderen/organisation-registry/commit/706610abe66cfbfc49b242258826474ef7fac1e8))
* allow force termination of kbo coupling ([943cc66](https://github.com/informatievlaanderen/organisation-registry/commit/943cc666112ca260fe33d0539c6343c9dc192b5a))
* allow OrganisationRegistryBeheerders to update organisations, even if terminated ([9e28567](https://github.com/informatievlaanderen/organisation-registry/commit/9e28567fee9f6c9200f46ddef7173a05aa2426cd))
* calculate fields to terminate ([49e2a46](https://github.com/informatievlaanderen/organisation-registry/commit/49e2a46dd1d335bd4207476ef835067b261413d9))
* clear current parent/building/... if necessary ([f31444d](https://github.com/informatievlaanderen/organisation-registry/commit/f31444d5f4e0e46120518652ee9de575147061d5))
* don't show edit buttons when org is terminated ([c37f73d](https://github.com/informatievlaanderen/organisation-registry/commit/c37f73d9ab7c8a1497ac7c0f93a14ff3eeeb4441))
* extract state into dedicated class ([cb8dcfb](https://github.com/informatievlaanderen/organisation-registry/commit/cb8dcfba3e86f87a22d62179d13b53cfb78e9b07))
* handle OrganisationTerminated in ElasticSearch projections ([797ad8c](https://github.com/informatievlaanderen/organisation-registry/commit/797ad8c190ad624362deaa49bf7edcd9c2765ff0))
* inject user into commands ([8587608](https://github.com/informatievlaanderen/organisation-registry/commit/8587608c038cb9fc2bf568fca7cb4a245af904f0))
* introduce IOrganisationField, IValidityBuilder interfaces ([104cb31](https://github.com/informatievlaanderen/organisation-registry/commit/104cb3142ef9ca41c01cb87a28c7ee1333e71850))
* optimize memory caches ([35b5e5b](https://github.com/informatievlaanderen/organisation-registry/commit/35b5e5bf4a19cac1cc2515e550d2521a417f2463))
* remove active/future main building/location logic ([f1d3a9d](https://github.com/informatievlaanderen/organisation-registry/commit/f1d3a9dd0294a57fae650fb2bb2f5e65227b6cc9))
* tell user if org is terminated according to kbo ([5ac05b9](https://github.com/informatievlaanderen/organisation-registry/commit/5ac05b90106bbbc0e655a403285bb22409fc1860))
* update capacities on OrganisationTerminated ([a939922](https://github.com/informatievlaanderen/organisation-registry/commit/a939922fed87bd2fce787bc08e3b8c2f2ec8ff5d))
* update jwt in integration tests ([4c72a6f](https://github.com/informatievlaanderen/organisation-registry/commit/4c72a6f4134dc3008e02ca2553d024d0c814129a))
* update org's validTo on OrganisationTerminated ([bc2db78](https://github.com/informatievlaanderen/organisation-registry/commit/bc2db788833fd2ef614edda4ae0c3bf3503f60c4))
* wip on terminating organisation ([915279f](https://github.com/informatievlaanderen/organisation-registry/commit/915279f274915ed003ba430479ad72917dbb7659))

## [1.63.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.63.0...v1.63.1) (2021-02-02)


### Bug Fixes

* move to 5.0.2 ([36a1d78](https://github.com/informatievlaanderen/organisation-registry/commit/36a1d785576dc19348e2fd0ce91ce6575eb27d10))

# [1.63.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.62.0...v1.63.0) (2020-12-24)


### Bug Fixes

* change how curl outputs to std and error ([95b8939](https://github.com/informatievlaanderen/organisation-registry/commit/95b8939e1a646f3eed2fa913f4bcc6f00c77daa3))


### Features

* reuse existing context instead of recreating ([e2b625f](https://github.com/informatievlaanderen/organisation-registry/commit/e2b625f1764345c536f3e2bbe7195926f0a66c69))

# [1.62.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.61.0...v1.62.0) (2020-12-23)


### Bug Fixes

* move to 5.0.1 ([cdcd237](https://github.com/informatievlaanderen/organisation-registry/commit/cdcd237ccaef3ed4e56dd23c3e342fac0b542960))


### Features

* use 2-way authentication for kbo ftps communication ([7d45ea6](https://github.com/informatievlaanderen/organisation-registry/commit/7d45ea6810b1c189a815c5d39f96996c583b23ac))

# [1.61.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.60.2...v1.61.0) (2020-12-16)


### Features

* hardcode scheme ([c8d2bcf](https://github.com/informatievlaanderen/organisation-registry/commit/c8d2bcf672256839fc0addf0cc57138a325d0319))

## [1.60.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.60.1...v1.60.2) (2020-11-18)


### Bug Fixes

* remove set-env usage in gh-actions ([bc4cc8d](https://github.com/informatievlaanderen/organisation-registry/commit/bc4cc8d98ba56e73c26460fe046d901b84c8625d))

## [1.60.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.60.0...v1.60.1) (2020-11-06)


### Bug Fixes

* logging ([bb0a808](https://github.com/informatievlaanderen/organisation-registry/commit/bb0a8083a743fcf02f649338228913fc934e0fb4))
* logging ([0e9574f](https://github.com/informatievlaanderen/organisation-registry/commit/0e9574f6252a1451539787a253a295602a2bb747))
* logging ([75b2587](https://github.com/informatievlaanderen/organisation-registry/commit/75b2587d0ece096ba1831350ddc15f69c72e932b))
* logging ([29f9e0f](https://github.com/informatievlaanderen/organisation-registry/commit/29f9e0f4ae590fadc3e4f22cdcf7127111125485))
* logging ([686bfef](https://github.com/informatievlaanderen/organisation-registry/commit/686bfefb8c3203ec4dca1bb5406518e073cc590f))
* logging ([5976557](https://github.com/informatievlaanderen/organisation-registry/commit/5976557c58c3f2e9ae78bd367ef871c54fe36e75))
* logging ([aa0a237](https://github.com/informatievlaanderen/organisation-registry/commit/aa0a2378cd58841225ff0e26ff2f0dd2e367d97e))
* logging ([e822f5c](https://github.com/informatievlaanderen/organisation-registry/commit/e822f5cda12dac02c0a0e173825c94854dcf18a9))
* logging ([4933081](https://github.com/informatievlaanderen/organisation-registry/commit/4933081da83481f83d6362f9ecce13055ee4e301))

# [1.60.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.59.0...v1.60.0) (2020-11-06)


### Features

* add example for user & pass settings in appsettings.json ([c1cb95c](https://github.com/informatievlaanderen/organisation-registry/commit/c1cb95c8276e220da011d101fa6b69fb6382537c))
* split up elastic connstring into read and write ([802f6a6](https://github.com/informatievlaanderen/organisation-registry/commit/802f6a65aaa866fc08f2fc8a596e3477926f6d8a))
* split up elastic user & pass into read and write ([e09b8e7](https://github.com/informatievlaanderen/organisation-registry/commit/e09b8e7f74ccda2ccc4186633a07a3f4c85ea795))

# [1.59.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.58.0...v1.59.0) (2020-11-02)


### Features

* expose x-search-metadata header ([ef068c6](https://github.com/informatievlaanderen/organisation-registry/commit/ef068c6910c16de44b8cecbb2f8a530a2c0f5896))

# [1.58.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.57.2...v1.58.0) (2020-10-30)


### Features

* add developer to people with rights for kbo termination ([0265c0e](https://github.com/informatievlaanderen/organisation-registry/commit/0265c0ec04b8576369132514fcc2fe859b70fbca))
* add explanation to functionality in kbo mgmt page ([fe0068b](https://github.com/informatievlaanderen/organisation-registry/commit/fe0068be6069603cd23a9d68028b6bc9994bcabe))
* add explanation to functionality in terminated orgs page ([1506cb3](https://github.com/informatievlaanderen/organisation-registry/commit/1506cb32462666222345f2cf11776cb0ca3da393))
* improve text by adding articles before KBO ([12cbd2e](https://github.com/informatievlaanderen/organisation-registry/commit/12cbd2eedce4d8b95ce249b92bb390ac01244604))
* rename title of kbo mgmt page ([3fa488f](https://github.com/informatievlaanderen/organisation-registry/commit/3fa488f173b6861cc4e634c487104c46f1dfa120))
* reword CannotCancelCouplingWithOrganisationCreatedFromKbo msg ([1a24d94](https://github.com/informatievlaanderen/organisation-registry/commit/1a24d942d80d0adda65575fd78ef5d8f703f694b))
* reword explanation in terminated orgs page ([77fefa1](https://github.com/informatievlaanderen/organisation-registry/commit/77fefa15c6824619909e7c4b068730122bf1d93e))

## [1.57.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.57.1...v1.57.2) (2020-10-21)


### Bug Fixes

* provide default value for newly required columns ([5068a9d](https://github.com/informatievlaanderen/organisation-registry/commit/5068a9df67a2b53b952f25fd9080f8f4fc5329ef))

## [1.57.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.57.0...v1.57.1) (2020-10-20)


### Bug Fixes

* use surname claim instead of now empty name claim ([82d7df2](https://github.com/informatievlaanderen/organisation-registry/commit/82d7df27a5e58a2e5efbf7cdf276f9b4ae7f3d88))

# [1.57.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.56.0...v1.57.0) (2020-10-20)


### Bug Fixes

* add missing organisationGuard arguments ([e02147b](https://github.com/informatievlaanderen/organisation-registry/commit/e02147be21c859eb347ea6fb53197e86d5293266))
* correct couple to kbo uri ([14c00fc](https://github.com/informatievlaanderen/organisation-registry/commit/14c00fcc421e223f5e8b647d6266e52eb15b677a))


### Features

* add alerts and clearer status info ([aac292a](https://github.com/informatievlaanderen/organisation-registry/commit/aac292ac7f4987a674851a93897f3b77a1831e0c))
* add backend for syncing kbo termination ([7acd7b3](https://github.com/informatievlaanderen/organisation-registry/commit/7acd7b3c64a7b88d46eac3b33b958e2f9e0790d7))
* add comments to the kbo controller methods ([bfa8111](https://github.com/informatievlaanderen/organisation-registry/commit/bfa81117a3b8e0f38acb350d2020a3f24b58264d))
* add ui to sync kbo termination ([87e1338](https://github.com/informatievlaanderen/organisation-registry/commit/87e13385863ae7df0be8a2e552d9e01f1284e745))
* allow cancelling of org created by kbo, after recoupling with kbo ([c77e7e8](https://github.com/informatievlaanderen/organisation-registry/commit/c77e7e8b4c0590da7769aa560b56fd6542c0db7a))
* allow syncing of end date of kbo coupling ([12abfbb](https://github.com/informatievlaanderen/organisation-registry/commit/12abfbbb03edc391324d4f30307163f9fcb8a060))
* change title ([828caae](https://github.com/informatievlaanderen/organisation-registry/commit/828caae5c8998bb48b3b9e319ed5af1c6db3ee97))
* clear kbo related data when syncing termination ([8f301b9](https://github.com/informatievlaanderen/organisation-registry/commit/8f301b9ab4331fd0d12315b4282a665da9c6ffa9))
* disable sync buttons when appropriate ([41dbd1b](https://github.com/informatievlaanderen/organisation-registry/commit/41dbd1bdaf6df83a1e8d0e1e6fa7145db8f0c51e))
* don't allow coupling with inactive kbo number ([c9fccf0](https://github.com/informatievlaanderen/organisation-registry/commit/c9fccf0f475fccb0fb4881c98dd55150622d13f3))
* don't manually fetch termination date, trust sync to set it ([8e36b4f](https://github.com/informatievlaanderen/organisation-registry/commit/8e36b4f047e7c35d3835060975e5e98d9e0e09d7))
* fetch organisation termination via magda ftp ([3f76671](https://github.com/informatievlaanderen/organisation-registry/commit/3f76671ccf15322b0216b6584833fcca2c7c2c22))
* improve overview title ([453e59f](https://github.com/informatievlaanderen/organisation-registry/commit/453e59f39b86559525ef28273e348fbb65d2db2f))
* improve title ([e098d77](https://github.com/informatievlaanderen/organisation-registry/commit/e098d77adf3a3af8576bb2c25b8ecd7e5e7c5ade))
* introduce separate event for manual syncing with kbo ([388e4f2](https://github.com/informatievlaanderen/organisation-registry/commit/388e4f252de072e7bb9ae9b934d5a4da79d90f1d))
* move kbo actions to dedicated controller ([9b71996](https://github.com/informatievlaanderen/organisation-registry/commit/9b719963f362cd2037862c8eff7ed865cab10ba2))
* only give org registry maintainer access to kbo sync/cancel features ([0fff7cd](https://github.com/informatievlaanderen/organisation-registry/commit/0fff7cd8ae8a6f0f00cac2657c3b4004cecc3944))
* prevent cancelling kbo coupling when organisation created from kbo ([843aaef](https://github.com/informatievlaanderen/organisation-registry/commit/843aaef0f75aefbdec5091991e717b36c23831c8))
* protect sync in ui from unauthorized access ([cdaeb58](https://github.com/informatievlaanderen/organisation-registry/commit/cdaeb588001ccabbac18b57d9c56fe90c3c0335b))
* reload the org after syncing ([e80ee8b](https://github.com/informatievlaanderen/organisation-registry/commit/e80ee8bc03c6ae6c102d2ee3dd56426eb2dd2ed9))
* remove item on termination instead of changing status ([3c42b7a](https://github.com/informatievlaanderen/organisation-registry/commit/3c42b7a0a1b594e000c0534e230a7085f11c2e15))
* rename 'sync' to 'manage' ([3208da0](https://github.com/informatievlaanderen/organisation-registry/commit/3208da0fd062307183b2b4463fc0ebc6ff9b5134))
* rename OrganisationCouplingWithKboTerminated ([fce1748](https://github.com/informatievlaanderen/organisation-registry/commit/fce174894263dc1d16351c0c129a865b600f3886))
* reorganise and add visual flair ([9a25e06](https://github.com/informatievlaanderen/organisation-registry/commit/9a25e062be00496d446d0316adeae3fa153a7b22))

# [1.56.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.55.0...v1.56.0) (2020-09-23)


### Bug Fixes

* move to 3.1.8 ([e626c60](https://github.com/informatievlaanderen/organisation-registry/commit/e626c60745d48a79c31fba360a8cf5b67cb2c214))


### Features

* put cancelling of kbo coupling behind toggle ([9b28c27](https://github.com/informatievlaanderen/organisation-registry/commit/9b28c2745b5ccaa749e7a2394ed813b8f2451eed))

# [1.55.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.54.0...v1.55.0) (2020-09-16)


### Features

* implement pkce flow ([f07a1e5](https://github.com/informatievlaanderen/organisation-registry/commit/f07a1e545010473e85b410778d3692ed8f756c48))
* remove verifier on signout ([c77446b](https://github.com/informatievlaanderen/organisation-registry/commit/c77446bc6c28e80b51f48c74cc14a09db367206c))

# [1.54.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.53.0...v1.54.0) (2020-09-15)


### Features

* allow the cancelling of a KBO coupling ([386e84f](https://github.com/informatievlaanderen/organisation-registry/commit/386e84f1ec575997b1efad6c4085047e0b374687))
* do not show archived name in LabelListItemView ([db2bb67](https://github.com/informatievlaanderen/organisation-registry/commit/db2bb6707fc0b8382daf6601ac623a9fb6f4eb56))
* improved info messages ([40f18de](https://github.com/informatievlaanderen/organisation-registry/commit/40f18de0fab39c4c60cf4d129849c47589ee9502))
* rename properties to clarify ([dd50e01](https://github.com/informatievlaanderen/organisation-registry/commit/dd50e01408bb93ecbb3f1adad471aee17bdcb92d))
* use consistent capitalization on 'KBO' ([b7b05e7](https://github.com/informatievlaanderen/organisation-registry/commit/b7b05e7c2681a48798e3db4b15214f792324074e))

# [1.53.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.52.2...v1.53.0) (2020-09-09)


### Features

* retry new kbo sync items first ([11bccb9](https://github.com/informatievlaanderen/organisation-registry/commit/11bccb912a123d438298f26ca5674dc20fa6a542))

## [1.52.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.52.1...v1.52.2) (2020-09-07)


### Bug Fixes

* only include organs actively linked to mep ([29a295e](https://github.com/informatievlaanderen/organisation-registry/commit/29a295e62854aa1b267fd4c24be7b4edc467bf82))

## [1.52.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.52.0...v1.52.1) (2020-08-31)


### Bug Fixes

* fix disposed object and make async ([182f491](https://github.com/informatievlaanderen/organisation-registry/commit/182f491d435163cf9ea33afe548ccbe2858a18b9))

# [1.52.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.51.0...v1.52.0) (2020-08-27)


### Features

* add sync from kbo to scheduler ([3ddcd36](https://github.com/informatievlaanderen/organisation-registry/commit/3ddcd36b4bb128bccca5329d7a6e4a411542f6e6))

# [1.51.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.50.0...v1.51.0) (2020-08-26)


### Features

* log unsuccessful magda responses ([5a966dd](https://github.com/informatievlaanderen/organisation-registry/commit/5a966ddac8a0e8a33ca5301b4555fe044c04dbd5))

# [1.50.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.49.0...v1.50.0) (2020-08-26)


### Features

* add resp minister and policy domain to participation summary ([3f37b90](https://github.com/informatievlaanderen/organisation-registry/commit/3f37b90cd9de182234ac0e60c5f6585d869adea8))
* recalculate percentages ([335658c](https://github.com/informatievlaanderen/organisation-registry/commit/335658cff6e36d4c96355e85e5b2cc39aad1bddd))

# [1.49.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.48.0...v1.49.0) (2020-08-24)


### Features

* only show bodies for mep in participation summary ([5f293e4](https://github.com/informatievlaanderen/organisation-registry/commit/5f293e43e098b26d2a49ce5596fb75ff55d8e07d))

# [1.48.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.47.0...v1.48.0) (2020-08-18)


### Features

* add application to reregister orgs with magda ([a971164](https://github.com/informatievlaanderen/organisation-registry/commit/a971164522d5ccd5912b17e226bd7d73aab58730))

# [1.47.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.46.0...v1.47.0) (2020-08-17)


### Features

* register inscription when calling kbo ([cda72ae](https://github.com/informatievlaanderen/organisation-registry/commit/cda72aeda90ff5cf7e6b12b982b07a880e2e6c3f))

# [1.46.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.45.0...v1.46.0) (2020-08-17)


### Features

* add error logging to CurlFtpsClient ([dc697ce](https://github.com/informatievlaanderen/organisation-registry/commit/dc697ce87003f1646addb935108203a1f8d7915c))
* enable tls encryption for kbo mutations ftp ([32ea60e](https://github.com/informatievlaanderen/organisation-registry/commit/32ea60e82e2713e7670a545f4ff1c774feb48491))
* use curl for ftps communications ([6be774a](https://github.com/informatievlaanderen/organisation-registry/commit/6be774abeccbae68824fbee6fb099bae350acb0f))
* use curl on PATH by default ([75ee787](https://github.com/informatievlaanderen/organisation-registry/commit/75ee7875d02eacb23ac5c00aad1bd5b3b8e6d6b0))

# [1.45.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.44.6...v1.45.0) (2020-07-20)


### Features

* enable tls encryption for kbo mutations ftp ([eb555dd](https://github.com/informatievlaanderen/organisation-registry/commit/eb555dd78ec359448be5a65c705b75dba621bbfd))

## [1.44.6](https://github.com/informatievlaanderen/organisation-registry/compare/v1.44.5...v1.44.6) (2020-07-19)


### Bug Fixes

* move to 3.1.6 ([0d3d129](https://github.com/informatievlaanderen/organisation-registry/commit/0d3d129bb7f242b6d9823c354def52e7dedc8fd1))

## [1.44.5](https://github.com/informatievlaanderen/organisation-registry/compare/v1.44.4...v1.44.5) (2020-07-07)

## [1.44.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.44.3...v1.44.4) (2020-06-25)


### Bug Fixes

* only include seats that are entitled to vote ([2f7bb1c](https://github.com/informatievlaanderen/organisation-registry/commit/2f7bb1c560d4b9ce518f7d44286a283d813cf19c))
* start calculating percentages from at least one ([0e4b3da](https://github.com/informatievlaanderen/organisation-registry/commit/0e4b3daa343980b133c92d7119515b3b1b0c636c))
* use separate totals when calculating separate percentages ([9399744](https://github.com/informatievlaanderen/organisation-registry/commit/9399744edc65c35cceea675458bac798d8c32539))

## [1.44.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.44.2...v1.44.3) (2020-06-25)


### Bug Fixes

* handle SeatTypeUpdated for mep ([469a4c9](https://github.com/informatievlaanderen/organisation-registry/commit/469a4c9bc65b77822db5790b01b26e39dc05d93b))
* only show active assignments ([fc13c0a](https://github.com/informatievlaanderen/organisation-registry/commit/fc13c0ab618f892210ebf5f68acce37405b23a18))

## [1.44.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.44.1...v1.44.2) (2020-06-19)


### Bug Fixes

* move to 3.1.5 ([5f3b8ba](https://github.com/informatievlaanderen/organisation-registry/commit/5f3b8ba7a24fb7952d37f6b4581f815267d2e49c))

## [1.44.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.44.0...v1.44.1) (2020-05-29)


### Bug Fixes

* update dependencies GRAR-752 ([7cdf314](https://github.com/informatievlaanderen/organisation-registry/commit/7cdf314e45bd0114d82461e953e6a072dfee2f7f))

# [1.44.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.43.5...v1.44.0) (2020-05-29)


### Bug Fixes

* remove typo in back to reports link ([a752990](https://github.com/informatievlaanderen/organisation-registry/commit/a7529902b70a61aa8d997a7be94d069bd0a0b306))


### Features

* add isEffective as column to seat type ([6b7ba9e](https://github.com/informatievlaanderen/organisation-registry/commit/6b7ba9e7d6f18fc8e64a24d056be83c66851e1b6))
* add participation summary report ([9f5b5a8](https://github.com/informatievlaanderen/organisation-registry/commit/9f5b5a889776815c1a7ff51b4a6ab9b7c25f1b08))
* allow updating seat type effectiveness from ui ([ed17370](https://github.com/informatievlaanderen/organisation-registry/commit/ed17370163482c510825f1c91dc2e44eff35c6b4))
* allow user to unset obligatory mep compliance ([70ba5a8](https://github.com/informatievlaanderen/organisation-registry/commit/70ba5a858673fe286e16616eab98f65d13340a43))
* calculate body participation grouped by isEffective ([9b768fb](https://github.com/informatievlaanderen/organisation-registry/commit/9b768fb49b49677840607369fffad0b65cbcb48a))
* calculate ff body participation grouped by isEffective ([ed4540a](https://github.com/informatievlaanderen/organisation-registry/commit/ed4540a66837478876773f4672f67f5c30646949))
* calculate male and female compliance server side ([37e13de](https://github.com/informatievlaanderen/organisation-registry/commit/37e13deede5987cf1ac07de9f1f411aaa588bba6))
* calculate totals based on body participations ([3ddd610](https://github.com/informatievlaanderen/organisation-registry/commit/3ddd6109645366282ca9b67e79d301d76ce4779d))
* introduce isEffective field to seat type ([3f4ddac](https://github.com/informatievlaanderen/organisation-registry/commit/3f4ddac89842ad58d1fbdb528ac6c3dc39120ade))
* make IsBalancedParticipationObligatory nullable ([3146967](https://github.com/informatievlaanderen/organisation-registry/commit/3146967e7a0dcc8f8b98473c57c6ae9335539065))
* make participation default to false when updating with null ([25fc8ed](https://github.com/informatievlaanderen/organisation-registry/commit/25fc8edd5e557aab9fa5f5b5d4282cfef1b1afc2))
* optimize participation summary report ([2c87df4](https://github.com/informatievlaanderen/organisation-registry/commit/2c87df49e9d3b6cd07635b66b42ec3d5d3ae386d))
* pass down mep compliance with body participations ([79f1c3b](https://github.com/informatievlaanderen/organisation-registry/commit/79f1c3b7641cd72815de1525e796390d38915967))
* remove colors from participation totals ([dccaf47](https://github.com/informatievlaanderen/organisation-registry/commit/dccaf47680a9670861c394d0356b345412f026f0))
* remove separate percentages from participation summary ([a58b31b](https://github.com/informatievlaanderen/organisation-registry/commit/a58b31b82e3e7e0ff04ad6e8145076710fd0ba41))
* show isEffective in seat type list ([d58ba76](https://github.com/informatievlaanderen/organisation-registry/commit/d58ba7681fe8abcaf4581e1c798c222fdf055326))
* show warning when total participation is nonCompliant ([7f83b33](https://github.com/informatievlaanderen/organisation-registry/commit/7f83b3308817327875f8fd39ee01d69f7f8eba14))


### Reverts

* Revert "chore: add dotnet ef to local tools" ([8670907](https://github.com/informatievlaanderen/organisation-registry/commit/8670907e6130c1d261e507e1ca60dbb4a3fc6ca7))

## [1.43.5](https://github.com/informatievlaanderen/organisation-registry/compare/v1.43.4...v1.43.5) (2020-05-25)


### Bug Fixes

* deal with npm packages ([0c7c3e6](https://github.com/informatievlaanderen/organisation-registry/commit/0c7c3e6e8872a338442fef139771ab5d9155db30))
* revert npm packages upgrade ([7457e73](https://github.com/informatievlaanderen/organisation-registry/commit/7457e73cc31b7f4a75350e0c69990f144d8729af))

## [1.43.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.43.3...v1.43.4) (2020-05-20)


### Bug Fixes

* push to aws ([e17bd5f](https://github.com/informatievlaanderen/organisation-registry/commit/e17bd5fec1367ab369d73c071e0669b6c183086c))

## [1.43.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.43.2...v1.43.3) (2020-05-20)


### Bug Fixes

* move to 3.1.4 ([3e4f33e](https://github.com/informatievlaanderen/organisation-registry/commit/3e4f33e57aaf3ffa8d1deed34479126d4749b7b1))

## [1.43.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.43.1...v1.43.2) (2020-05-20)


### Bug Fixes

* add window prop for aiv uri ([f5c390e](https://github.com/informatievlaanderen/organisation-registry/commit/f5c390e2a2f16cfc40cde1077af784e8313953a6))

## [1.43.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.43.0...v1.43.1) (2020-05-20)


### Bug Fixes

* force build ([8f9915f](https://github.com/informatievlaanderen/organisation-registry/commit/8f9915fbe30786feed24d44a930d59293cbe009c))

# [1.43.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.42.0...v1.43.0) (2020-05-20)


### Features

* create index with settings from config ([61e001c](https://github.com/informatievlaanderen/organisation-registry/commit/61e001c2a66c1cd623e2118158fc44d58dd821ef))
* upgrade es in docker compose ([24a632c](https://github.com/informatievlaanderen/organisation-registry/commit/24a632c8b4627f3923c5e68493d17340beac6cee))

# [1.42.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.41.2...v1.42.0) (2020-05-18)


### Features

* add ssXXX format to handle dates like 2019-12-24T16:01:22+05:45 ([edd3344](https://github.com/informatievlaanderen/organisation-registry/commit/edd3344f74151df235367acd0f3ee98fe9e47573))
* upgrade to ES 7 ([ecaaeb4](https://github.com/informatievlaanderen/organisation-registry/commit/ecaaeb45ff1a5cacedb71a6f79f7c52202ab9052))
* upgrade to NEST 7.6.1 ([3d66e95](https://github.com/informatievlaanderen/organisation-registry/commit/3d66e95ca39c72302950b95af13b03df958562b8))

## [1.41.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.41.1...v1.41.2) (2020-04-30)


### Bug Fixes

* empty bin and obj folders ([511eae8](https://github.com/informatievlaanderen/organisation-registry/commit/511eae8313769b86fead6c9abd3df70ab2940b14))

## [1.41.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.41.0...v1.41.1) (2020-04-29)


### Bug Fixes

* remove municipality copy pasted docs ([e3fb18a](https://github.com/informatievlaanderen/organisation-registry/commit/e3fb18ac9df872936b35632367088358ec69c250))

# [1.41.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.40.0...v1.41.0) (2020-04-29)


### Bug Fixes

* build website with npm run build ([89f2240](https://github.com/informatievlaanderen/organisation-registry/commit/89f2240e9cd27b5002e18646d9bd6f6effe44b42))
* don't generate manually supplied guid ([9967baa](https://github.com/informatievlaanderen/organisation-registry/commit/9967baabf452b97c8becd8f8336dad483d46ad70))
* don't generate manually supplied guid ([5ed071f](https://github.com/informatievlaanderen/organisation-registry/commit/5ed071fd744423db22d65145598ac1219fd277b8))
* dont delete node modules ([7a935d9](https://github.com/informatievlaanderen/organisation-registry/commit/7a935d9bf2c99ee1fce7c0d23fd1bf14d090c437))
* ef no longer evaluates linq queries client side ([6db2264](https://github.com/informatievlaanderen/organisation-registry/commit/6db226404647e8a440e9a1799282ec1888492fdd))
* inline query to make ef support it ([117a098](https://github.com/informatievlaanderen/organisation-registry/commit/117a0985c6a1069178a2e66b9d491bde9852be49))
* make more fields nullable ([7e046f2](https://github.com/informatievlaanderen/organisation-registry/commit/7e046f2960fcb50a36e12f2223602b150de36102))
* make query inmemory to bypass current efcore limitations ([7a2bb38](https://github.com/informatievlaanderen/organisation-registry/commit/7a2bb388984ebb0c94c2136c79a4f9af6fa2b6a2))
* make source nullable ([dd8f8e5](https://github.com/informatievlaanderen/organisation-registry/commit/dd8f8e56e10c593f7e9cd10b2760dfadaf33176d))
* move to gh actions ([ea8dbf6](https://github.com/informatievlaanderen/organisation-registry/commit/ea8dbf67312426169a0a84ad4436c82b3f6a0516))
* paket.lock out of date ([2a6957e](https://github.com/informatievlaanderen/organisation-registry/commit/2a6957e0b234bac4c98f9817d7614df4a4c8ba81))
* pass build number correctly ([8b7f00f](https://github.com/informatievlaanderen/organisation-registry/commit/8b7f00f47882f0adff38ff0ec5838201a1ad6060))
* prevent wrong generation of column name by making FK more explicit ([b457eb0](https://github.com/informatievlaanderen/organisation-registry/commit/b457eb0e5b36a28701ff25a6d3dd7dd96731f798))
* prevent wrong generation of column name by making FK more explicit ([84b216d](https://github.com/informatievlaanderen/organisation-registry/commit/84b216dae0867135ce04c1a80a853c64a283e1dd))
* remove ambiguity for ef linq statements ([d03f373](https://github.com/informatievlaanderen/organisation-registry/commit/d03f3731417be167e861869e8f30440de675c0a3))
* remove npm test ([2ec20f3](https://github.com/informatievlaanderen/organisation-registry/commit/2ec20f320ba58c6483fdf90e8eeb3438f769065f))
* remove npm version which builds the site ([15a7da0](https://github.com/informatievlaanderen/organisation-registry/commit/15a7da0ec524ffd2b18871d62ef6872ed7d73b06))
* temp patch semantic ([f050267](https://github.com/informatievlaanderen/organisation-registry/commit/f050267cb6c4b62dbdc803baeb7545776501f74e))
* use correct name property ([cb09cf5](https://github.com/informatievlaanderen/organisation-registry/commit/cb09cf584f6517cc61005742b021a3ecb8b63418))


### Features

* allow enabling/disabling of lock via config ([75222b5](https://github.com/informatievlaanderen/organisation-registry/commit/75222b5e5f25c4233a5eecfabedeabb4feb26dd2))
* commit paket.lock ([058936f](https://github.com/informatievlaanderen/organisation-registry/commit/058936f6df8dbd5b62b624366348c742f499f3fd))
* fix build script ([f2904b6](https://github.com/informatievlaanderen/organisation-registry/commit/f2904b637f519babbcf477c991f34ca692e3955b))
* make commands and events async ([59e9f99](https://github.com/informatievlaanderen/organisation-registry/commit/59e9f99b2ff53117af069719c5f20eac18cae6be))
* make db-nullable fields dotnet-nullable ([175af2e](https://github.com/informatievlaanderen/organisation-registry/commit/175af2e8070bc7192674e035cb21ebe01e31e7eb))
* make db-nullable fields dotnet-nullable ([6e621bb](https://github.com/informatievlaanderen/organisation-registry/commit/6e621bb680d7f004afc80a82e333e1dbec05fa87))
* make processing reactions async ([278ddc2](https://github.com/informatievlaanderen/organisation-registry/commit/278ddc2267fd9085156c312a67941cb2397014d7))
* replace distributedmutex with pkg (WIP) ([d50afda](https://github.com/informatievlaanderen/organisation-registry/commit/d50afda24900924492c5fc22ee056203a5fb98f0))
* update packages to fix problemdetails exception ([204051f](https://github.com/informatievlaanderen/organisation-registry/commit/204051f15d8a6167f7eb31651fef0d1a70ddd438))
* upgrade aspnetcore packages ([7e65f8d](https://github.com/informatievlaanderen/organisation-registry/commit/7e65f8db32415f5a2ddb9af786540f2cf1a501ba))
* upgrade basisregisters packages ([86f4080](https://github.com/informatievlaanderen/organisation-registry/commit/86f40806590e6e979c20f14f73575cdb062f5efa))
* upgrade brotli package ([50da1c4](https://github.com/informatievlaanderen/organisation-registry/commit/50da1c42f85518dcf13b9ff6fef625e26eeac239))
* upgrade build.pipeline to 4.0.5 ([df962a5](https://github.com/informatievlaanderen/organisation-registry/commit/df962a5afed551513ffa571e426e9007634c0180))
* upgrade code analysis package ([ca9d6d2](https://github.com/informatievlaanderen/organisation-registry/commit/ca9d6d2ee4dd947f9dcf1c53db0125d47a1ebeb0))
* upgrade crypto package ([d124ef9](https://github.com/informatievlaanderen/organisation-registry/commit/d124ef9559548150fc6f9dd9609baaa9f6e134b0))
* upgrade csvhelper package ([fff559c](https://github.com/informatievlaanderen/organisation-registry/commit/fff559ca1400b33800380277cde1b97916b65b1b))
* upgrade dockerfile runtime to 3.1.2 ([b09fd7c](https://github.com/informatievlaanderen/organisation-registry/commit/b09fd7c58e64d926d09ef79b67f07272e272bce9))
* upgrade epplus package ([b9f97a5](https://github.com/informatievlaanderen/organisation-registry/commit/b9f97a5825fbfe13eeba1535bb8615887b37669a))
* upgrade fluentassertions package ([610dfa9](https://github.com/informatievlaanderen/organisation-registry/commit/610dfa903b5e8d4f3c8e39dbe76330702a6736f8))
* upgrade fluentftp package ([ea194e3](https://github.com/informatievlaanderen/organisation-registry/commit/ea194e34cdae089cb67f631bc6b15597d3fecbc9))
* upgrade healthchecks package ([4a86a21](https://github.com/informatievlaanderen/organisation-registry/commit/4a86a21ed6fae23f12b2109bd41afd51e18bc2b4))
* upgrade identitymodel packages ([728f5bc](https://github.com/informatievlaanderen/organisation-registry/commit/728f5bc456a50b56d6d9a6afb3b4b4c84ff8cbbc))
* upgrade iisintegration package ([3decebb](https://github.com/informatievlaanderen/organisation-registry/commit/3decebbdf3e3f8917a2fe045d8f443480b067f92))
* upgrade jwt bearer package ([d4fd4ca](https://github.com/informatievlaanderen/organisation-registry/commit/d4fd4ca95be543d15b70ba11f851d7e73006620b))
* upgrade main identitymodel package ([e096e7a](https://github.com/informatievlaanderen/organisation-registry/commit/e096e7a44cad0d40cac0e619be0589648aaee486))
* upgrade packages and projects to net3.1 ([d557dd6](https://github.com/informatievlaanderen/organisation-registry/commit/d557dd6d67918250498770e40e59cfb0e77a6d7f))
* upgrade sdk version ([10f18f1](https://github.com/informatievlaanderen/organisation-registry/commit/10f18f187c624454c389d2ac5d772be1a88008b2))
* upgrade serilog ES package ([858bca6](https://github.com/informatievlaanderen/organisation-registry/commit/858bca60feb5603062fd5be4ce043bfdc06d72f3))
* upgrade servicemodel packages ([5975597](https://github.com/informatievlaanderen/organisation-registry/commit/59755974885e46ef0fbb48285693fc827aa6fea5))
* upgrade testhost and ef packages ([e1155ab](https://github.com/informatievlaanderen/organisation-registry/commit/e1155ab408e4186daa2d762fd2007347d26c1799))
* use IContextFactory in memory cache ([df6a871](https://github.com/informatievlaanderen/organisation-registry/commit/df6a8714ed8920e6eb5219cb8734cd459c6b872c))


### Reverts

* Revert "feat: upgrade brotli package" ([96e91da](https://github.com/informatievlaanderen/organisation-registry/commit/96e91da64eba584eb97cb36abd158c85ab94c33c))
* Revert "feat: upgrade serilog ES package" ([53749e0](https://github.com/informatievlaanderen/organisation-registry/commit/53749e0a464a7a1344f7509ae3d54f08b9a8ab48))

# [1.41.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.40.0...v1.41.0) (2020-04-29)


### Bug Fixes

* build website with npm run build ([89f2240](https://github.com/informatievlaanderen/organisation-registry/commit/89f2240e9cd27b5002e18646d9bd6f6effe44b42))
* don't generate manually supplied guid ([9967baa](https://github.com/informatievlaanderen/organisation-registry/commit/9967baabf452b97c8becd8f8336dad483d46ad70))
* don't generate manually supplied guid ([5ed071f](https://github.com/informatievlaanderen/organisation-registry/commit/5ed071fd744423db22d65145598ac1219fd277b8))
* ef no longer evaluates linq queries client side ([6db2264](https://github.com/informatievlaanderen/organisation-registry/commit/6db226404647e8a440e9a1799282ec1888492fdd))
* inline query to make ef support it ([117a098](https://github.com/informatievlaanderen/organisation-registry/commit/117a0985c6a1069178a2e66b9d491bde9852be49))
* make more fields nullable ([7e046f2](https://github.com/informatievlaanderen/organisation-registry/commit/7e046f2960fcb50a36e12f2223602b150de36102))
* make query inmemory to bypass current efcore limitations ([7a2bb38](https://github.com/informatievlaanderen/organisation-registry/commit/7a2bb388984ebb0c94c2136c79a4f9af6fa2b6a2))
* make source nullable ([dd8f8e5](https://github.com/informatievlaanderen/organisation-registry/commit/dd8f8e56e10c593f7e9cd10b2760dfadaf33176d))
* move to gh actions ([ea8dbf6](https://github.com/informatievlaanderen/organisation-registry/commit/ea8dbf67312426169a0a84ad4436c82b3f6a0516))
* paket.lock out of date ([2a6957e](https://github.com/informatievlaanderen/organisation-registry/commit/2a6957e0b234bac4c98f9817d7614df4a4c8ba81))
* pass build number correctly ([8b7f00f](https://github.com/informatievlaanderen/organisation-registry/commit/8b7f00f47882f0adff38ff0ec5838201a1ad6060))
* prevent wrong generation of column name by making FK more explicit ([b457eb0](https://github.com/informatievlaanderen/organisation-registry/commit/b457eb0e5b36a28701ff25a6d3dd7dd96731f798))
* prevent wrong generation of column name by making FK more explicit ([84b216d](https://github.com/informatievlaanderen/organisation-registry/commit/84b216dae0867135ce04c1a80a853c64a283e1dd))
* remove ambiguity for ef linq statements ([d03f373](https://github.com/informatievlaanderen/organisation-registry/commit/d03f3731417be167e861869e8f30440de675c0a3))
* remove npm test ([2ec20f3](https://github.com/informatievlaanderen/organisation-registry/commit/2ec20f320ba58c6483fdf90e8eeb3438f769065f))
* remove npm version which builds the site ([15a7da0](https://github.com/informatievlaanderen/organisation-registry/commit/15a7da0ec524ffd2b18871d62ef6872ed7d73b06))
* use correct name property ([cb09cf5](https://github.com/informatievlaanderen/organisation-registry/commit/cb09cf584f6517cc61005742b021a3ecb8b63418))


### Features

* allow enabling/disabling of lock via config ([75222b5](https://github.com/informatievlaanderen/organisation-registry/commit/75222b5e5f25c4233a5eecfabedeabb4feb26dd2))
* commit paket.lock ([058936f](https://github.com/informatievlaanderen/organisation-registry/commit/058936f6df8dbd5b62b624366348c742f499f3fd))
* fix build script ([f2904b6](https://github.com/informatievlaanderen/organisation-registry/commit/f2904b637f519babbcf477c991f34ca692e3955b))
* make commands and events async ([59e9f99](https://github.com/informatievlaanderen/organisation-registry/commit/59e9f99b2ff53117af069719c5f20eac18cae6be))
* make db-nullable fields dotnet-nullable ([175af2e](https://github.com/informatievlaanderen/organisation-registry/commit/175af2e8070bc7192674e035cb21ebe01e31e7eb))
* make db-nullable fields dotnet-nullable ([6e621bb](https://github.com/informatievlaanderen/organisation-registry/commit/6e621bb680d7f004afc80a82e333e1dbec05fa87))
* make processing reactions async ([278ddc2](https://github.com/informatievlaanderen/organisation-registry/commit/278ddc2267fd9085156c312a67941cb2397014d7))
* replace distributedmutex with pkg (WIP) ([d50afda](https://github.com/informatievlaanderen/organisation-registry/commit/d50afda24900924492c5fc22ee056203a5fb98f0))
* update packages to fix problemdetails exception ([204051f](https://github.com/informatievlaanderen/organisation-registry/commit/204051f15d8a6167f7eb31651fef0d1a70ddd438))
* upgrade aspnetcore packages ([7e65f8d](https://github.com/informatievlaanderen/organisation-registry/commit/7e65f8db32415f5a2ddb9af786540f2cf1a501ba))
* upgrade basisregisters packages ([86f4080](https://github.com/informatievlaanderen/organisation-registry/commit/86f40806590e6e979c20f14f73575cdb062f5efa))
* upgrade brotli package ([50da1c4](https://github.com/informatievlaanderen/organisation-registry/commit/50da1c42f85518dcf13b9ff6fef625e26eeac239))
* upgrade build.pipeline to 4.0.5 ([df962a5](https://github.com/informatievlaanderen/organisation-registry/commit/df962a5afed551513ffa571e426e9007634c0180))
* upgrade code analysis package ([ca9d6d2](https://github.com/informatievlaanderen/organisation-registry/commit/ca9d6d2ee4dd947f9dcf1c53db0125d47a1ebeb0))
* upgrade crypto package ([d124ef9](https://github.com/informatievlaanderen/organisation-registry/commit/d124ef9559548150fc6f9dd9609baaa9f6e134b0))
* upgrade csvhelper package ([fff559c](https://github.com/informatievlaanderen/organisation-registry/commit/fff559ca1400b33800380277cde1b97916b65b1b))
* upgrade dockerfile runtime to 3.1.2 ([b09fd7c](https://github.com/informatievlaanderen/organisation-registry/commit/b09fd7c58e64d926d09ef79b67f07272e272bce9))
* upgrade epplus package ([b9f97a5](https://github.com/informatievlaanderen/organisation-registry/commit/b9f97a5825fbfe13eeba1535bb8615887b37669a))
* upgrade fluentassertions package ([610dfa9](https://github.com/informatievlaanderen/organisation-registry/commit/610dfa903b5e8d4f3c8e39dbe76330702a6736f8))
* upgrade fluentftp package ([ea194e3](https://github.com/informatievlaanderen/organisation-registry/commit/ea194e34cdae089cb67f631bc6b15597d3fecbc9))
* upgrade healthchecks package ([4a86a21](https://github.com/informatievlaanderen/organisation-registry/commit/4a86a21ed6fae23f12b2109bd41afd51e18bc2b4))
* upgrade identitymodel packages ([728f5bc](https://github.com/informatievlaanderen/organisation-registry/commit/728f5bc456a50b56d6d9a6afb3b4b4c84ff8cbbc))
* upgrade iisintegration package ([3decebb](https://github.com/informatievlaanderen/organisation-registry/commit/3decebbdf3e3f8917a2fe045d8f443480b067f92))
* upgrade jwt bearer package ([d4fd4ca](https://github.com/informatievlaanderen/organisation-registry/commit/d4fd4ca95be543d15b70ba11f851d7e73006620b))
* upgrade main identitymodel package ([e096e7a](https://github.com/informatievlaanderen/organisation-registry/commit/e096e7a44cad0d40cac0e619be0589648aaee486))
* upgrade packages and projects to net3.1 ([d557dd6](https://github.com/informatievlaanderen/organisation-registry/commit/d557dd6d67918250498770e40e59cfb0e77a6d7f))
* upgrade sdk version ([10f18f1](https://github.com/informatievlaanderen/organisation-registry/commit/10f18f187c624454c389d2ac5d772be1a88008b2))
* upgrade serilog ES package ([858bca6](https://github.com/informatievlaanderen/organisation-registry/commit/858bca60feb5603062fd5be4ce043bfdc06d72f3))
* upgrade servicemodel packages ([5975597](https://github.com/informatievlaanderen/organisation-registry/commit/59755974885e46ef0fbb48285693fc827aa6fea5))
* upgrade testhost and ef packages ([e1155ab](https://github.com/informatievlaanderen/organisation-registry/commit/e1155ab408e4186daa2d762fd2007347d26c1799))
* use IContextFactory in memory cache ([df6a871](https://github.com/informatievlaanderen/organisation-registry/commit/df6a8714ed8920e6eb5219cb8734cd459c6b872c))


### Reverts

* Revert "feat: upgrade brotli package" ([96e91da](https://github.com/informatievlaanderen/organisation-registry/commit/96e91da64eba584eb97cb36abd158c85ab94c33c))
* Revert "feat: upgrade serilog ES package" ([53749e0](https://github.com/informatievlaanderen/organisation-registry/commit/53749e0a464a7a1344f7509ae3d54f08b9a8ab48))

# [1.40.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.39.0...v1.40.0) (2020-04-15)


### Features

* clear sync info on success ([5ae2cae](https://github.com/informatievlaanderen/organisation-registry/commit/5ae2caeadbf3a07453db380bd8632a7c1f1fc04b))
* log ftp events ([0ea23bb](https://github.com/informatievlaanderen/organisation-registry/commit/0ea23bb05067600b19890e45fc0b0ba88b0d633f))

# [1.39.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.38.0...v1.39.0) (2020-04-10)


### Features

* log exception when kbo call fails ([68ddcb1](https://github.com/informatievlaanderen/organisation-registry/commit/68ddcb1b8be976ad6fc1a122b2d82268ce214b5b))

# [1.38.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.37.0...v1.38.0) (2020-04-10)


### Features

* provide better exception for already coupled orgs ([5a746f2](https://github.com/informatievlaanderen/organisation-registry/commit/5a746f2945ed0ba47637b8749c63bae7c413520b))

# [1.37.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.36.1...v1.37.0) (2020-04-07)


### Features

* log external ip when running kbo mutations ([9cd083e](https://github.com/informatievlaanderen/organisation-registry/commit/9cd083e571cecbeb40d53edbb91837035aba62de))
* set better defaults for logging ([80ea5ac](https://github.com/informatievlaanderen/organisation-registry/commit/80ea5ac591a12e6fc4d887422d1b8afa375b89a1))

## [1.36.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.36.0...v1.36.1) (2020-04-06)


### Bug Fixes

* copy kbo mutations dockerfile in csproj ([152d27b](https://github.com/informatievlaanderen/organisation-registry/commit/152d27b5ee57a2756a8550332d6aa28836c7fd61))

# [1.36.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.35.0...v1.36.0) (2020-04-06)


### Features

* add docker and init file ([55a6233](https://github.com/informatievlaanderen/organisation-registry/commit/55a623395c5c9f2bcf834d035d045cfc81b20b1d))

# [1.35.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.34.0...v1.35.0) (2020-04-03)


### Features

* update packages according to npm audit ([ec6dd0a](https://github.com/informatievlaanderen/organisation-registry/commit/ec6dd0a1fc9f29ddc47b48c7a0a0e6897fc44593))

# [1.34.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.33.1...v1.34.0) (2020-04-02)


### Features

* commit updated package-lock after revert ([d6957ed](https://github.com/informatievlaanderen/organisation-registry/commit/d6957ed616b253dd0051c4addc8156a19c64f560))
* lock versions with shrinkwrap ([7a57672](https://github.com/informatievlaanderen/organisation-registry/commit/7a576724ccd926cef80d927cfa69b58ce3a6c9d8))

## [1.33.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.33.0...v1.33.1) (2020-04-01)


### Bug Fixes

* don't trim spaces when deserializing events ([6b875f0](https://github.com/informatievlaanderen/organisation-registry/commit/6b875f0fed845f5883b817b228d3d1e931d66b2f))

# [1.33.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.32.0...v1.33.0) (2020-04-01)


### Bug Fixes

* use form to include disabled fields ([880d905](https://github.com/informatievlaanderen/organisation-registry/commit/880d90599bdb00cd2f268cd2115adad7f83d1295))


### Features

* update uris to acm changes of 2020-03-12 ([f896dd1](https://github.com/informatievlaanderen/organisation-registry/commit/f896dd1cd0ba8f54f56414c725ecb898229d36ec))

# [1.32.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.31.0...v1.32.0) (2020-03-27)


### Features

* show previous org name in label list after coupling with kbo ([15a8d8f](https://github.com/informatievlaanderen/organisation-registry/commit/15a8d8f7caeb6b8addf9984544f516eb903b92b1))

# [1.31.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.30.1...v1.31.0) (2020-03-25)


### Features

* refactor and add organisation tests ([d38a92d](https://github.com/informatievlaanderen/organisation-registry/commit/d38a92d5f45442c9461d361b1615bc1483388c62))

## [1.30.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.30.0...v1.30.1) (2020-03-24)


### Bug Fixes

* prevent from coupling to more than one organisation ([8353d68](https://github.com/informatievlaanderen/organisation-registry/commit/8353d68e7b0b4147bea9a148c348755f9b703fbd))

# [1.30.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.29.0...v1.30.0) (2020-03-24)


### Features

* make magda exceptions more visible ([c695e74](https://github.com/informatievlaanderen/organisation-registry/commit/c695e745ff789bd8f8795bcb1c3def29d02c9116))
* show kbo error message in exceptionz ([f150267](https://github.com/informatievlaanderen/organisation-registry/commit/f150267a6c847222f6fab9d5f20a9f15a57b3271))
* take non-sepa nr when iban and accountnr are null ([a980d33](https://github.com/informatievlaanderen/organisation-registry/commit/a980d33e993add1869f4c28f7ce8563295a001b7))

# [1.29.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.28.0...v1.29.0) (2020-03-23)


### Features

* make es tests dependant on env var ([e3a4140](https://github.com/informatievlaanderen/organisation-registry/commit/e3a41405c3a3e2901b0545bee9656727b8c6924e))
* re-add es tests ([963fd6d](https://github.com/informatievlaanderen/organisation-registry/commit/963fd6d00ac94a6a74d90320df38aafb932f5851))

# [1.28.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.27.0...v1.28.0) (2020-03-23)


### Bug Fixes

* handle legal form being removed after update ([5a9c80f](https://github.com/informatievlaanderen/organisation-registry/commit/5a9c80f2e551701a0c8819c74738e56e590ae2fd))


### Features

* don't publish info has changed when (short)name didn't change ([6c84103](https://github.com/informatievlaanderen/organisation-registry/commit/6c8410364b09ff8450df20665ebbe57dd1d81bf6))
* use account number if no iban was provided ([e21e3b9](https://github.com/informatievlaanderen/organisation-registry/commit/e21e3b9de1583e2bb81665ef5616dfd3d18f2bbc))

# [1.27.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.26.0...v1.27.0) (2020-03-20)


### Bug Fixes

* use owned instance ([730eabb](https://github.com/informatievlaanderen/organisation-registry/commit/730eabbb0ba0b876144adeb80922cc3c2cc704b2))


### Features

* add logging ([b9aaa94](https://github.com/informatievlaanderen/organisation-registry/commit/b9aaa9483f7773470738ae55d007ca462e52628a))

# [1.26.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.25.0...v1.26.0) (2020-03-20)


### Features

* add kbo sync marker event ([db10994](https://github.com/informatievlaanderen/organisation-registry/commit/db10994ff17703836f51f07f11a321a379964fc2))
* add migration to separate status from info ([8e8b2fd](https://github.com/informatievlaanderen/organisation-registry/commit/8e8b2fde716d53c48980796ac72d0601e8ca8519))
* clarify kbo sync item properties ([5bb839f](https://github.com/informatievlaanderen/organisation-registry/commit/5bb839f2fbecf1d3d2be83de1dc32d22dc2854ae))
* put limit on kbo sync ([8697d5e](https://github.com/informatievlaanderen/organisation-registry/commit/8697d5ea834e41de6d3c3626925e2471dab4b761))

# [1.25.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.24.0...v1.25.0) (2020-03-19)


### Features

* use mutex for kbo mutations fetcher ([2a4d4d0](https://github.com/informatievlaanderen/organisation-registry/commit/2a4d4d07a4211b996be21321bbe25ff2683fe505))

# [1.24.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.23.0...v1.24.0) (2020-03-18)


### Bug Fixes

* copy kbo mutations to production ([440c183](https://github.com/informatievlaanderen/organisation-registry/commit/440c183c44a6bab03fbf5ff965841ec20ba2b538))
* force build ([07760bb](https://github.com/informatievlaanderen/organisation-registry/commit/07760bb833d48a3e1f83b3b52166790c88f02f27))
* make async ([ffb26f7](https://github.com/informatievlaanderen/organisation-registry/commit/ffb26f7a9f8461bd194c385b8d06acd20e0d94ec))


### Features

* include kbo mutations in build script ([1e7609a](https://github.com/informatievlaanderen/organisation-registry/commit/1e7609a5ad8ed0e145f3a30b0bf3d22e17f79cd4))
* use IHttpClientFactory for magda calls ([d67cada](https://github.com/informatievlaanderen/organisation-registry/commit/d67cadab53cdee35f8656de7e8c14952ae4dfbb0))

# [1.23.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.22.2...v1.23.0) (2020-03-18)


### Bug Fixes

* change build to new user ([fd05169](https://github.com/informatievlaanderen/organisation-registry/commit/fd051690d6cfe23bdf53cd3ad775e2604caa83fb))
* decrease whitespace in menu when bank accounts not visible ([cc261d2](https://github.com/informatievlaanderen/organisation-registry/commit/cc261d235b819ee1eb3fa71798d6f047abbc0c88))
* use context factory ([a7288da](https://github.com/informatievlaanderen/organisation-registry/commit/a7288da2f8014e1046ad37ae2c99b4746b5d32a4))


### Features

* complete domain logic for update from kbo ([9d4d16d](https://github.com/informatievlaanderen/organisation-registry/commit/9d4d16da91011e641ee67577ac354ea3239d73fd))
* handle kbo removed events ([fb7a18f](https://github.com/informatievlaanderen/organisation-registry/commit/fb7a18fb0ff414a0d78eb672d255126eab6aac7b))
* make sure bank accounts are also fetched from magda ([cb8bc86](https://github.com/informatievlaanderen/organisation-registry/commit/cb8bc864f37448924c4b5a36a7e72caa34757c6d))
* only show couple button when allowed to edit ([bacb9cb](https://github.com/informatievlaanderen/organisation-registry/commit/bacb9cbdcd268b804ae27a08561da3d18ab50dfb))
* prevent accidental commit of personal data ([5860b41](https://github.com/informatievlaanderen/organisation-registry/commit/5860b410992a5a6d84648ad71624b3f4b4db8534))
* show external ip in developer config endpoint ([06a03b0](https://github.com/informatievlaanderen/organisation-registry/commit/06a03b0313b743daff8b0079c6987b41a629f84f))
* trim names coming from kbo ([b097a76](https://github.com/informatievlaanderen/organisation-registry/commit/b097a76a04db030a0e81290395e5b5db3cea042d))
* use factory to create ftp client ([be30bcf](https://github.com/informatievlaanderen/organisation-registry/commit/be30bcf35e9bf84e4b362430e679ffcc0a71af6b))

## [1.22.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.22.1...v1.22.2) (2020-03-16)


### Bug Fixes

* disable editing kbo locations ([c64103b](https://github.com/informatievlaanderen/organisation-registry/commit/c64103b64705c80d328d4b5da02023022e5e46fb))

## [1.22.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.22.0...v1.22.1) (2020-03-13)


### Bug Fixes

* force build ([8f7db8d](https://github.com/informatievlaanderen/organisation-registry/commit/8f7db8d814e13e9cbc26df4fc527e48340b3dcbf))

# [1.22.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.21.0...v1.22.0) (2020-03-13)


### Features

* make kbo data readonly ([64ffeb1](https://github.com/informatievlaanderen/organisation-registry/commit/64ffeb158bf523e017bc3fd2b72846b2bbc10e9a))

# [1.21.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.20.0...v1.21.0) (2020-03-12)


### Features

* allow lack of legal form ([e0df61a](https://github.com/informatievlaanderen/organisation-registry/commit/e0df61a128a0d262eaba1d8d72e1a0cd4ebbea80))
* make magda responses testable ([c2d1c1e](https://github.com/informatievlaanderen/organisation-registry/commit/c2d1c1e20367f12cbbde4e526eb90cd05cc7eba2))

# [1.20.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.19.0...v1.20.0) (2020-03-11)


### Features

* apply postel's law to magda data ([fe0d29c](https://github.com/informatievlaanderen/organisation-registry/commit/fe0d29c2ef1edc8add8a02fb4755d4725c4f26aa))

# [1.19.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.18.0...v1.19.0) (2020-03-10)


### Features

* throw if no kbo org found ([e62cdf8](https://github.com/informatievlaanderen/organisation-registry/commit/e62cdf8f3203fa664102b219f1de958d973f378a))
* use GetAwaiter ([83ca019](https://github.com/informatievlaanderen/organisation-registry/commit/83ca0198829865decc49c2ae6daa95774fceff5a))

# [1.18.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.17.0...v1.18.0) (2020-03-10)


### Bug Fixes

* remove invalid ref ([4a46ed6](https://github.com/informatievlaanderen/organisation-registry/commit/4a46ed62ca218fb738726d9720d97cc6012be45b))


### Features

* show kbo nr on info page ([0b6c526](https://github.com/informatievlaanderen/organisation-registry/commit/0b6c526d3c0aa8ac9495f5101f8b3c29d62a09c2))
* split kbo from non-kbo data in aggregate ([129e05b](https://github.com/informatievlaanderen/organisation-registry/commit/129e05be95ca4b42ae9a0c9c82e238c8f58cb8ed))

# [1.17.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.16.0...v1.17.0) (2020-03-10)


### Features

* add kbo number as prop on ES projections ([e913fdb](https://github.com/informatievlaanderen/organisation-registry/commit/e913fdba23a6cc46133977d95af5ebeb9bb3aede))
* don't add key for kbo ([bbfad04](https://github.com/informatievlaanderen/organisation-registry/commit/bbfad04a286494628d891f95ffd461bc3621f0ab))
* use kbo number prop on org ([fdf0b5b](https://github.com/informatievlaanderen/organisation-registry/commit/fdf0b5b92336dea4ab0005b3bf1448f84f12f542))

# [1.16.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.15.0...v1.16.0) (2020-03-05)


### Features

* use new kbo field for uniqueness ([e8f79c8](https://github.com/informatievlaanderen/organisation-registry/commit/e8f79c8e80909d556a946f54f748a893fed3d6d4))

# [1.15.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.14.7...v1.15.0) (2020-03-05)


### Bug Fixes

* match response models to actual response ([93a5654](https://github.com/informatievlaanderen/organisation-registry/commit/93a56545b0eeb1e493d738a31c674d07464b2a37))
* more rework from previous repo ([9e09e4a](https://github.com/informatievlaanderen/organisation-registry/commit/9e09e4a238678fd8e6939031f9c895fc20237594))
* rework coupling to kbo after copying from prev repo ([2cf4e17](https://github.com/informatievlaanderen/organisation-registry/commit/2cf4e17dd2e509a8f85fcef144ccc2b5c86f002d))
* update tests after code migration ([5dc5b59](https://github.com/informatievlaanderen/organisation-registry/commit/5dc5b59a6fb3114fc26d2ab11e577896f1a513fe))


### Features

* apply OrganisationCoupledWithKbo ([4fbc135](https://github.com/informatievlaanderen/organisation-registry/commit/4fbc1351c054f4bdb9099e8dde678a46fce7b9b7))
* don't redirect uris when requested not to ([9b53bee](https://github.com/informatievlaanderen/organisation-registry/commit/9b53bee2004ad808d8321b37190b9b3e7a958a6b))
* migrate kbo sync code from agiv repo ([56f30de](https://github.com/informatievlaanderen/organisation-registry/commit/56f30de77503bf102ccec2f8256ebe956ea16570))
* only get one legal form from kbo ([901b08a](https://github.com/informatievlaanderen/organisation-registry/commit/901b08a638072761516de3b2a038fe2e8173e33c))

## [1.14.7](https://github.com/informatievlaanderen/organisation-registry/compare/v1.14.6...v1.14.7) (2020-03-03)


### Bug Fixes

* update dockerid detection ([720d495](https://github.com/informatievlaanderen/organisation-registry/commit/720d4952fe0cf116ac0e3cae64b8fa1b6da5199e))

## [1.14.6](https://github.com/informatievlaanderen/organisation-registry/compare/v1.14.5...v1.14.6) (2020-02-25)


### Bug Fixes

* add support for format filters ([c938440](https://github.com/informatievlaanderen/organisation-registry/commit/c938440e0a127d1dbc1b72377a6b970426518b3a))
* rename request param to match with client ([56a09a6](https://github.com/informatievlaanderen/organisation-registry/commit/56a09a607baaee91fa61632b1df5b8b9a78a3dfc))

## [1.14.5](https://github.com/informatievlaanderen/organisation-registry/compare/v1.14.4...v1.14.5) (2020-02-24)


### Bug Fixes

* add missing location fields ([c6edffb](https://github.com/informatievlaanderen/organisation-registry/commit/c6edffbca0e28617336344625fbdeb1385d30bb0))

## [1.14.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.14.3...v1.14.4) (2020-02-22)


### Bug Fixes

* provide logging factory ([866a8fa](https://github.com/informatievlaanderen/organisation-registry/commit/866a8fab709ceaaa06f18c17f8e8bc316a4d22fe))

## [1.14.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.14.2...v1.14.3) (2020-02-20)


### Bug Fixes

* add missing classifications fields ([165a86e](https://github.com/informatievlaanderen/organisation-registry/commit/165a86eff4b1db636d28bb5754294874fa080c12))

## [1.14.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.14.1...v1.14.2) (2020-02-19)


### Bug Fixes

* add IsEditable placeholder ([eefd13f](https://github.com/informatievlaanderen/organisation-registry/commit/eefd13fa19ac75cf013ba0e463f3535dffdab896))

## [1.14.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.14.0...v1.14.1) (2020-02-18)


### Bug Fixes

* add missing label fields ([1fb617e](https://github.com/informatievlaanderen/organisation-registry/commit/1fb617e3cde38b824a7be9d0ca752ba67fbec445))

# [1.14.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.13.0...v1.14.0) (2020-02-18)


### Bug Fixes

* add missing bank account fields ([7b5a879](https://github.com/informatievlaanderen/organisation-registry/commit/7b5a8798ad06f38753d899c58b8d7624b0f238f1))


### Features

* use dev uris in launchSettings ([d72fa9d](https://github.com/informatievlaanderen/organisation-registry/commit/d72fa9df58e4e0d03dbde690a733b96f74a4bc79))

# [1.13.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.12.0...v1.13.0) (2020-02-17)


### Bug Fixes

* don't show inactive results in mep report ([2ec7776](https://github.com/informatievlaanderen/organisation-registry/commit/2ec7776a027674a0f2c75f99eaa336a0e8812a08))


### Features

* allow setting max retry attempts in config ([3589cb6](https://github.com/informatievlaanderen/organisation-registry/commit/3589cb68453219c79432112cd94c2136996164d6))
* log runner name ([67257fe](https://github.com/informatievlaanderen/organisation-registry/commit/67257fe64c8162721363fb4c27b63f561857f1b3))
* set ES batch size via params ([8c02d44](https://github.com/informatievlaanderen/organisation-registry/commit/8c02d446e71c0b83b43287f276deb5cccbc4a52e))

# [1.12.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.11.0...v1.12.0) (2020-02-17)


### Features

* pass original exception to ES exception ([e84b0c6](https://github.com/informatievlaanderen/organisation-registry/commit/e84b0c637fe5e87de5a0a0171ec8f8b606031ac5))

# [1.11.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.10.2...v1.11.0) (2020-02-11)


### Features

* use base64 string for Kbo cert ([0e00d22](https://github.com/informatievlaanderen/organisation-registry/commit/0e00d2265662283a45d165e40b34c6157dcf7272))

## [1.10.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.10.1...v1.10.2) (2020-02-07)


### Bug Fixes

* use the formal framework parent org ([56ed2ec](https://github.com/informatievlaanderen/organisation-registry/commit/56ed2ecb9c156bce98c1a9795d9597b7ad792122))

## [1.10.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.10.0...v1.10.1) (2020-02-03)


### Bug Fixes

* make FormalFrameworkOrganisationReport more testable ([ff4ccf4](https://github.com/informatievlaanderen/organisation-registry/commit/ff4ccf4d4a2b7d9fa189c1a898d3ad406dda205d))
* use only the currently active org classifications ([29827c9](https://github.com/informatievlaanderen/organisation-registry/commit/29827c9cfd783a3b9c32642aa6a38207cf3f0d3a))

# [1.10.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.9.2...v1.10.0) (2020-01-28)


### Bug Fixes

* force build ([8b296e1](https://github.com/informatievlaanderen/organisation-registry/commit/8b296e19e16ea8f8f22b3c8a6fa349b97a9070dd))
* increase no_output_timeout ([236e3c2](https://github.com/informatievlaanderen/organisation-registry/commit/236e3c227c9bd2ff4ed6e35cf1a43388b8568cce))


### Features

* re-enable api integration tests ([3a94e40](https://github.com/informatievlaanderen/organisation-registry/commit/3a94e40af3de723401fafae915edb049944afea9))

## [1.9.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.9.1...v1.9.2) (2020-01-10)


### Bug Fixes

* use optional guids for body mandate requests ([11d3397](https://github.com/informatievlaanderen/organisation-registry/commit/11d339704fa26474868acf42f9795f41f46a8a5f))
* use OR json options in mvc ([58e6d63](https://github.com/informatievlaanderen/organisation-registry/commit/58e6d634fea7c7ba13d66643b474311df114a11f))

## [1.9.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.9.0...v1.9.1) (2019-12-27)


### Bug Fixes

* don't nest query ([a28f77b](https://github.com/informatievlaanderen/organisation-registry/commit/a28f77bac98a82d13c6743b02c3487e7148ff96d))

# [1.9.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.8.2...v1.9.0) (2019-12-24)


### Bug Fixes

* remove hardcoded literate console logging ([1dfb24a](https://github.com/informatievlaanderen/organisation-registry/commit/1dfb24a0e10fd092561dcbdf5970011a794d10dc))


### Features

* add mutex to ftp dump ([7a5bf4c](https://github.com/informatievlaanderen/organisation-registry/commit/7a5bf4c0762e0f4fc3c36c398b04863f3933f7e9))
* add mutex to vlaanderenbenotifier ([fdac74c](https://github.com/informatievlaanderen/organisation-registry/commit/fdac74cc7b61be1992057025befaf8ace02ed6e4))

## [1.8.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.8.1...v1.8.2) (2019-12-20)

## [1.8.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.8.0...v1.8.1) (2019-12-20)


### Bug Fixes

* use OR serialization settings ([cbd9b87](https://github.com/informatievlaanderen/organisation-registry/commit/cbd9b8762dd68c2a112ca623387cc2ea2a5ec6ad))

# [1.8.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.7.0...v1.8.0) (2019-12-19)


### Bug Fixes

* make logging setup consistent, pass ILoggerFactory ([1591307](https://github.com/informatievlaanderen/organisation-registry/commit/159130755c213a7e70e5cfb0a0d457aef8449072))


### Features

* allow anonymous connection if no user provided ([37e2781](https://github.com/informatievlaanderen/organisation-registry/commit/37e2781f3aa61ecd9b19a3c3e3f03af9339a14d3))
* use mutex to prevent concurring processing in elasticsearch runner ([376f425](https://github.com/informatievlaanderen/organisation-registry/commit/376f42533b44f876457f737fa5a946fffb1f5fc8))
* use mutex to prevent concurring processing in reporting runner ([c6090b9](https://github.com/informatievlaanderen/organisation-registry/commit/c6090b977666e41bf52bce1444f2766631cf400d))

# [1.7.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.6.2...v1.7.0) (2019-12-18)


### Bug Fixes

* don't run dynamo tests unless env var is set ([387e7e2](https://github.com/informatievlaanderen/organisation-registry/commit/387e7e22e6ffe2e955a379663028befedc36bb72))


### Features

* add DistributedMutex ([eb1b876](https://github.com/informatievlaanderen/organisation-registry/commit/eb1b876f615a590f8b93b77c49856fe0b467c8fa))
* print settings on run ([9b33be5](https://github.com/informatievlaanderen/organisation-registry/commit/9b33be5287cacfdd552ea6ddb788655848e64aea))
* use mutex to prevent concurring processing ([ef0db79](https://github.com/informatievlaanderen/organisation-registry/commit/ef0db792e10eceffab369b4e3ed36ae4dd6ea0ae))

## [1.6.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.6.1...v1.6.2) (2019-12-17)


### Bug Fixes

* prevent flood of localization warnings in logging ([f592f5a](https://github.com/informatievlaanderen/organisation-registry/commit/f592f5aa11ee033631f099cac6b4b3de92e50253))

## [1.6.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.6.0...v1.6.1) (2019-12-16)


### Bug Fixes

* push scheduler to production ([fe6e5ad](https://github.com/informatievlaanderen/organisation-registry/commit/fe6e5ad1e9a2ffedb2b82a54add88e97906e794c))

# [1.6.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.5.1...v1.6.0) (2019-12-16)


### Features

* add scheduler to docker build ([8419123](https://github.com/informatievlaanderen/organisation-registry/commit/841912392ff76ed009b996e7d850e97daabb9df8))

## [1.5.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.5.0...v1.5.1) (2019-12-13)


### Bug Fixes

* configure logging to provide ILoggerFactory sooner ([0007d34](https://github.com/informatievlaanderen/organisation-registry/commit/0007d34c1f4db89a07fa8e5613ba6b4338a5389e))
* copy any files in workspace ([cc3625f](https://github.com/informatievlaanderen/organisation-registry/commit/cc3625f68b68ff20fe0a610025509a18d6ced7c2))
* create semver file manually ([46c832b](https://github.com/informatievlaanderen/organisation-registry/commit/46c832be4eda2304e9db54ae575e600424a8529d))
* create workspace dir ([97a0e17](https://github.com/informatievlaanderen/organisation-registry/commit/97a0e170c6c61bfcfdea32d52ef2a8a5a62cfa44))
* use correct syntax ([f006587](https://github.com/informatievlaanderen/organisation-registry/commit/f006587c071bd76be5e55db3f7cbad72402f6a2d))

# [1.5.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.4.0...v1.5.0) (2019-12-13)


### Features

* default log levels to Information ([855578b](https://github.com/informatievlaanderen/organisation-registry/commit/855578bc76b33df23d5222b43c95101a44993689))

# [1.4.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.5...v1.4.0) (2019-12-12)


### Features

* add scheduler image ([272252e](https://github.com/informatievlaanderen/organisation-registry/commit/272252ed3659320c2b9fbab216684320f5ab90f2))

## [1.3.5](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.4...v1.3.5) (2019-12-11)


### Bug Fixes

* rename roles consistently ([9abae8b](https://github.com/informatievlaanderen/organisation-registry/commit/9abae8b8778b477756c12a129929c27031179587))

## [1.3.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.3...v1.3.4) (2019-12-10)


### Bug Fixes

* add missing kboNumber form control ([53a81f2](https://github.com/informatievlaanderen/organisation-registry/commit/53a81f203e672ab95926b79a776c6c2ce9b2a58c))

## [1.3.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.2...v1.3.3) (2019-12-09)


### Bug Fixes

* redirect using $http_host ([c6b76db](https://github.com/informatievlaanderen/organisation-registry/commit/c6b76dba19d68d5f8b6755bf09c9e7d19c7c5a7d))

## [1.3.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.1...v1.3.2) (2019-12-06)


### Bug Fixes

* coalesce empty string for FormalFrameworkName ([6d0bd70](https://github.com/informatievlaanderen/organisation-registry/commit/6d0bd70a5e584f2f0ec4cf0d3204f83d883d9516))

## [1.3.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.3.0...v1.3.1) (2019-12-06)


### Bug Fixes

* use serilog values for logging ([28c3b85](https://github.com/informatievlaanderen/organisation-registry/commit/28c3b8507685124c1ff2f1d0fafa9d3a914e806a))

# [1.3.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.2.0...v1.3.0) (2019-12-06)


### Features

* also oic exception log to logger ([ab72bf6](https://github.com/informatievlaanderen/organisation-registry/commit/ab72bf6733adf2c5a7d9112e423ed388dd35a874))

# [1.2.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.1.1...v1.2.0) (2019-12-06)


### Features

* increase log level ([9b65709](https://github.com/informatievlaanderen/organisation-registry/commit/9b65709f06fa2cfe73adc3a127e88dd1029f1ed2))
* provide more info on oic exchange failure ([056115d](https://github.com/informatievlaanderen/organisation-registry/commit/056115d3f9a2415924092e7969d134a641325a79))

## [1.1.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.1.0...v1.1.1) (2019-12-06)


### Bug Fixes

* push docs to nuget DC-24 ([937d911](https://github.com/informatievlaanderen/organisation-registry/commit/937d911d81b4b4817b1f5e014840e7650611ea3a))
* rename listview ([1ac6fb6](https://github.com/informatievlaanderen/organisation-registry/commit/1ac6fb6ac5edabbd47a7f92dc4e5d1889af1c969))

# [1.1.0](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.6...v1.1.0) (2019-12-03)


### Bug Fixes

* add paket.template for public api DC-24 ([fad7259](https://github.com/informatievlaanderen/organisation-registry/commit/fad72598d0c3b80018b18a25553c5fe17bcd3b78))
* add paket.template for public api DC-24 ([20d4971](https://github.com/informatievlaanderen/organisation-registry/commit/20d4971e11cc7a78c5da3f823e65aadf25c01f13))
* push docker containers to production ([f9b344f](https://github.com/informatievlaanderen/organisation-registry/commit/f9b344f775e314e2c00708cc26ac983f8f5c682e))


### Features

* dockerize AgentschapZorgEnGezondheid.FtpDump DC-7 ([fbbc877](https://github.com/informatievlaanderen/organisation-registry/commit/fbbc87702ec0e860695f86f0743260c6c19b24ba))
* dockerize ElasticSearch.Projections DC-9 ([15d7f0b](https://github.com/informatievlaanderen/organisation-registry/commit/15d7f0b96ce8bda71146c8e80e6f1c10e090e2a9))
* dockerize Projections.Delegations DC-10 ([e4787d3](https://github.com/informatievlaanderen/organisation-registry/commit/e4787d3de858681593de72e2e63feff5cabbbb27))
* dockerize Projections.Reporting DC-11 ([a63ba9a](https://github.com/informatievlaanderen/organisation-registry/commit/a63ba9a0a26b76526f6835e8f02d4319a5a589d5))
* dockerize VlaanderenBeNotifier DC-8 ([77d364e](https://github.com/informatievlaanderen/organisation-registry/commit/77d364e86bd2bc1d7bd83abddfdc7a44a6bd2088))

## [1.0.6](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.5...v1.0.6) (2019-12-03)


### Bug Fixes

* use correct header names ([e622c42](https://github.com/informatievlaanderen/organisation-registry/commit/e622c42330441b0529b7b209fcf527f9144ea9b2))

## [1.0.5](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.4...v1.0.5) (2019-12-03)


### Bug Fixes

* use correct api url ([c49bfb8](https://github.com/informatievlaanderen/organisation-registry/commit/c49bfb8a01816e932a8fc4c636bcb30eb4a5ee05))

## [1.0.4](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.3...v1.0.4) (2019-12-03)


### Bug Fixes

* correct title before angular loads ([92dc130](https://github.com/informatievlaanderen/organisation-registry/commit/92dc130e20a38f9a491dbd6e2e9f6fb4a02939d7))

## [1.0.3](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.2...v1.0.3) (2019-12-02)


### Bug Fixes

* use 9xxx port range ([b882889](https://github.com/informatievlaanderen/organisation-registry/commit/b8828897100dc8fd9803ddc3f6a74346a82cf7a8))

## [1.0.2](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.1...v1.0.2) (2019-12-02)


### Bug Fixes

* dont push non existent docker images ([0cbe6c0](https://github.com/informatievlaanderen/organisation-registry/commit/0cbe6c0add7c8a8274b9dc85e3443e1e169d07c9))

## [1.0.1](https://github.com/informatievlaanderen/organisation-registry/compare/v1.0.0...v1.0.1) (2019-12-02)


### Bug Fixes

* dont push non existent nugets ([6a33912](https://github.com/informatievlaanderen/organisation-registry/commit/6a339127e77b9b6c21a8f68b5bb3ac546e686db6))

# 1.0.0 (2019-12-02)


### Bug Fixes

* add circleci support ([92ba7e3](https://github.com/informatievlaanderen/organisation-registry/commit/92ba7e39775744717dbcfcbcc1c06a2226735c9d))
* add json compact serilog ([ec81271](https://github.com/informatievlaanderen/organisation-registry/commit/ec8127198b000231edb45d3bcbe319b4dcad3d2f))
* add missing values for tests ([8555520](https://github.com/informatievlaanderen/organisation-registry/commit/85555201e1b8f9a0de92beb715b253cdbde4c164))
* copy appsettings ([83155fa](https://github.com/informatievlaanderen/organisation-registry/commit/83155faae9e3c94fcf931ef1e12dd35ddcb9defe))
* make sure Url.Action still works ([80c0177](https://github.com/informatievlaanderen/organisation-registry/commit/80c01776a02804f0e57f0fd532eaa2dd7794c25e))
* only build api and site ([1090e3d](https://github.com/informatievlaanderen/organisation-registry/commit/1090e3d38dea520cda4c7aed28d9dbd126d852e4))
* point to correct default api endpoint ([58b87f9](https://github.com/informatievlaanderen/organisation-registry/commit/58b87f932e9186412f32d6c1fda6e933348e02ba))
* redirect oic to hash oic ([d468208](https://github.com/informatievlaanderen/organisation-registry/commit/d468208238271680802b04e10b95381cdeaa5d4f))
* redirect to idp on auth request ([1e2da40](https://github.com/informatievlaanderen/organisation-registry/commit/1e2da407f6f303240b6f1c1886ad50a6bfe0f004))
* remap port of sql server ([9d15098](https://github.com/informatievlaanderen/organisation-registry/commit/9d1509830f57d04ac5b5edacc242aa4ad5b71261))
* remove unnecessary comma ([edfe11b](https://github.com/informatievlaanderen/organisation-registry/commit/edfe11b9409ed4d865d717f22c1f49d71bf68c0e))
* rename ef context ([c8151e3](https://github.com/informatievlaanderen/organisation-registry/commit/c8151e3657d8a89f5a60425763b370a13d1954f5))
* require appsettings ([1db0710](https://github.com/informatievlaanderen/organisation-registry/commit/1db0710b67e99b8c0b3b7637d002b7846dd50eb8))
* run sql on default port ([bd60593](https://github.com/informatievlaanderen/organisation-registry/commit/bd6059385070a7f0d03dca46f139ed99086be6f4))
* run tests against localhost ([0ba8bf1](https://github.com/informatievlaanderen/organisation-registry/commit/0ba8bf124a5d37310e368bbe364f504419570878))
* see if build works without pack ([bd4f40a](https://github.com/informatievlaanderen/organisation-registry/commit/bd4f40a54f56197eb589aec2b3589d364defaeac))
* see if build works without tests ([df5f0c8](https://github.com/informatievlaanderen/organisation-registry/commit/df5f0c8b82c6586d2bec0f8cd76e7b9f7fd5a070))
* title is Wegwijs ([eda763a](https://github.com/informatievlaanderen/organisation-registry/commit/eda763af91313e9b2ecb4e3581843044bedc9726))
* use different connstrings for fixtures ([babddd0](https://github.com/informatievlaanderen/organisation-registry/commit/babddd06712016e7b184c1687453b40eb84d3dec))
* use hasAnyOfRoles ([14d54c4](https://github.com/informatievlaanderen/organisation-registry/commit/14d54c480fcc0ef63209bec676a5565be47f7a40))
* use local docker db as default ([41da3db](https://github.com/informatievlaanderen/organisation-registry/commit/41da3db76489d9b1b9a63e1cbb3329cfb72117b4))
* use nullable GUIDs for optional values ([d756739](https://github.com/informatievlaanderen/organisation-registry/commit/d756739cd24a0373db4ed5bd74e5937b3136f64e))
* use staging uris as default ([15d9d6c](https://github.com/informatievlaanderen/organisation-registry/commit/15d9d6ce0f5e9de69e03ae4f9834b48ed87c27e9))


### Features

* add agentschap zorg en gezondheid ([bf77a0b](https://github.com/informatievlaanderen/organisation-registry/commit/bf77a0bd9b34ea640e153e753f8cce3ba27b24df))
* add api ([7289d19](https://github.com/informatievlaanderen/organisation-registry/commit/7289d19ad57ad2e7eaff12984ae91819490a53ff))
* add command timeout ([58348f7](https://github.com/informatievlaanderen/organisation-registry/commit/58348f7c88097174bac5193a7652c12976b23220))
* add compensating action runner, not in sln! ([62423a8](https://github.com/informatievlaanderen/organisation-registry/commit/62423a84b8720b66e04c8683aecfbdfa9e77a5f1))
* add configuration db ([511a8f8](https://github.com/informatievlaanderen/organisation-registry/commit/511a8f8fe79fc4a0118b5f97cae65129d816dbb4))
* add default cors ([143ef58](https://github.com/informatievlaanderen/organisation-registry/commit/143ef58734af15df85cac9b7f7cee2b04ecba4ed))
* add default datadog settings ([77d0f3b](https://github.com/informatievlaanderen/organisation-registry/commit/77d0f3bac1d733ae4696e74e674ce32432de8d54))
* add delegation projections ([5c70117](https://github.com/informatievlaanderen/organisation-registry/commit/5c7011711956e9914ae6c1de74047ac166ae7304))
* add domain ([b913220](https://github.com/informatievlaanderen/organisation-registry/commit/b913220394ecec959f9b9ea405c844f708f967ac))
* add elasticsearch ([e4434bf](https://github.com/informatievlaanderen/organisation-registry/commit/e4434bf8277bb1dca6369d5adb4a8b364eef9a2c))
* add es projections ([f61c4ca](https://github.com/informatievlaanderen/organisation-registry/commit/f61c4cabf89c4501e0513e9757050530f7c41a88))
* add event rewriter, not in sln! ([2164dd0](https://github.com/informatievlaanderen/organisation-registry/commit/2164dd0881e46b69310640a979ff69d48a6d8a7e))
* add file splitter ([70caa4b](https://github.com/informatievlaanderen/organisation-registry/commit/70caa4b492579ae2c6c18395962a88781a8ea140))
* add infrastructure ([803e7e6](https://github.com/informatievlaanderen/organisation-registry/commit/803e7e6b66a939dc4823dbd2bf69487f7c17abff))
* add magda wcf ([607e132](https://github.com/informatievlaanderen/organisation-registry/commit/607e132a1e24755a321d9253a7842436a1d55a6d))
* add reporting projections ([355a4bb](https://github.com/informatievlaanderen/organisation-registry/commit/355a4bbf011f4a2c52dcd14e3886a55e97ffac04))
* add sql projection tables ([96a9a5d](https://github.com/informatievlaanderen/organisation-registry/commit/96a9a5deceff68a9d58c2f14f0d88147ecad361d))
* add tests ([eaede7f](https://github.com/informatievlaanderen/organisation-registry/commit/eaede7f7cb830aabb7fd631e2f3818542cb39c9a))
* add ui ([b7069b5](https://github.com/informatievlaanderen/organisation-registry/commit/b7069b58c92a0740beee05ab577d41da7ce3e0cb))
* add upgrader for namespace move ([d62fde8](https://github.com/informatievlaanderen/organisation-registry/commit/d62fde86b674118be87b421920f320399b5cc023))
* add vlaanderen be notifier ([30e0284](https://github.com/informatievlaanderen/organisation-registry/commit/30e028485b520046ee6b29ba17627e0c9fee62fe))
* developer is always 'in role' ([0f17c3d](https://github.com/informatievlaanderen/organisation-registry/commit/0f17c3de8e5efa22fde5b69e231f93bcf2997b4b))
* remove appInsights ([bfd9053](https://github.com/informatievlaanderen/organisation-registry/commit/bfd9053f30358e4c662da5e9db743b7100c9e9e2))
* remove unused AuthSettings ([caa77bc](https://github.com/informatievlaanderen/organisation-registry/commit/caa77bc6c6298b4c34799de51bb2ed7e6086b651))
* serialize events by full name ([456b11f](https://github.com/informatievlaanderen/organisation-registry/commit/456b11ff83d301093f9ed7c3a78c9f72b907f047))
* use empty cert when configured cert can't be found ([474b77f](https://github.com/informatievlaanderen/organisation-registry/commit/474b77f91eb64d361af418e3d60815ff0e30f620))
