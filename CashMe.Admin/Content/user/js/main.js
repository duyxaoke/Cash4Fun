    /*
    * ----------------------------------------------------------------------------------------
        Author       : miniboss
        Template Name: jhon - Personal Portfolio Template
        Version      : 1.0
    * ----------------------------------------------------------------------------------------
    */

    (function(){
        "use strict";

    /*
         * ----------------------------------------------------------------------------------------
         *  active owl-carousel
         * ----------------------------------------------------------------------------------------
    */
        $('.clients_says').owlCarousel({
            loop:true,
            margin:10,
            autoplayTimeout:3000,
            smartSpeed:450,
            animateOut: "fadeOut",
            animateIn: "slideInUp",
            autoplay:true,
            stagePadding:30,
            nav: false,
            dots:true,
            navText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right"></i>'],

            responsive:{
            0:{
                items:1
            },
            600:{
                items:1
            },
            1000:{
                items:1
            }
        }
});

    /*
         * ----------------------------------------------------------------------------------------
         *  smooth scroll
         * ----------------------------------------------------------------------------------------
    */
        $("a").on('click', function(event) {
            if (this.hash !== "") {
                event.preventDefault();
                var hash = this.hash;
            $('html, body').animate({
                scrollTop: $(hash).offset().top
                }, 1200, function(){
                window.location.hash = hash;
          });
        }
      });
    /*
         * ----------------------------------------------------------------------------------------
         *  sticky menu
         * ----------------------------------------------------------------------------------------
    */
        $(window).on('scroll',function(){
            if($(this).scrollTop() > 50){
            $('.navigation_area').addClass('sticky');
        }
        else{
            $('.navigation_area').removeClass('sticky');
    }
  });
        /*----------active mixitup------------*/
        $('.portfolio_items').mixItUp();
    /*
         * ----------------------------------------------------------------------------------------
         *  active venubox
         * ----------------------------------------------------------------------------------------
    */
        $('.venobox_design').venobox({
            framewidth: '430px',
            frameheight: '430px',
            bgcolor: '#5dff5e',
            titleattr: 'data-title',
            numeratio: true,
            infinigall: true,
            spinner:'wave',
            overlayColor:'rgba(0,0,0,0.96)',
            closeBackground:'transparent',
            closeColor:'#333',
            titlePosition:'bottom'
    });

    /*
         * ----------------------------------------------------------------------------------------
         *  active YTPlayer
         * ----------------------------------------------------------------------------------------
    */
        jQuery(".user_quote").YTPlayer();

    /*
         * ----------------------------------------------------------------------------------------
         *  responsive menu
         * ----------------------------------------------------------------------------------------

 //    */
        function mobileMenu(){
                var windwidth = $(window).width();
                    if(windwidth < 768){
                    $('.main_menu').hide();

                    $('.mobile_menu').on('click',function(){

                    $('.main_menu').slideToggle("slow");
                });
                    $('.main_menu .nav > li > a').on('click',function(){
                    $('.main_menu').hide();
            });
         }
        }
        mobileMenu();
        $(window).on('resize',function() {
            mobileMenu();
        });

    /*
         * ----------------------------------------------------------------------------------------
         *  scroll to top
         * ----------------------------------------------------------------------------------------
    */
        $('.scrolltp').fadeOut(1000);
            $(window).on('scroll', function(){

            if($(this).scrollTop()>500){
                 $('.scrolltp').fadeIn(1000);
              }
              else
              {
                 $('.scrolltp').fadeOut(1000);
              }
         });
        $('.scrolltp').on('click', function(){
        $('html,body').animate({scrollTop: 0}, 2000);
     });

     jQuery(window).load(function() {
         $('.preloader').fadeOut('slow');
     });


 })(jQuery);
